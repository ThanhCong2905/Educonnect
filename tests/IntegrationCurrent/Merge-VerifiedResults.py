import pathlib
import re
import xml.etree.ElementTree as ET
from datetime import datetime

root = pathlib.Path(__file__).resolve().parents[2] / "TestResults" / "Integration"
main_path = root / "Integration.trx"
main = ET.parse(main_path)
tree = main.getroot()
ns = {"m": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
tag = lambda name: "{" + ns["m"] + "}" + name
results = tree.find(tag("Results"))
case_by_id = {
    match.group(): result
    for result in results
    if (match := re.search(r"IT-\d{3}", result.get("testName", "")))
}

finish = datetime.fromisoformat(tree.find(tag("Times")).get("finish").replace("Z", "+00:00")).timestamp()
sources = sorted((path for path in root.glob("Single-*.trx") if path.stat().st_mtime >= finish - 2), key=lambda path: path.stat().st_mtime)
merged = set()
for source in sources:
    test_run = ET.parse(source).getroot()
    source_results = test_run.find(tag("Results"))
    if source_results is None:
        continue
    for result in source_results:
        match = re.search(r"IT-\d{3}", result.get("testName", ""))
        if match and match.group() in case_by_id:
            previous = case_by_id[match.group()]
            replacement = ET.fromstring(ET.tostring(result))
            index = list(results).index(previous)
            results.remove(previous)
            results.insert(index, replacement)
            case_by_id[match.group()] = replacement
            merged.add(match.group())

outcomes = [result.get("outcome") for result in results]
counters = tree.find(".//" + tag("Counters"))
counters.set("total", str(len(outcomes)))
counters.set("executed", str(sum(value != "NotExecuted" for value in outcomes)))
counters.set("passed", str(outcomes.count("Passed")))
counters.set("failed", str(outcomes.count("Failed")))
counters.set("notExecuted", str(outcomes.count("NotExecuted")))
summary = tree.find(tag("ResultSummary"))
summary.set("outcome", "Failed" if "Failed" in outcomes else "Completed")
main.write(main_path, encoding="utf-8", xml_declaration=True)
print(f"Merged {len(merged)} filtered case IDs; {len(outcomes)} total, {outcomes.count('Passed')} pass, {outcomes.count('Failed')} fail, {outcomes.count('NotExecuted')} skipped")
