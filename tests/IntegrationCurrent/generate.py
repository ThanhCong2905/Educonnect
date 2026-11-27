import json,pathlib,re
p=pathlib.Path(__file__).parent
d=json.loads((p/'Manifest.json').read_text(encoding='utf-8-sig'))
handlers={}
for filename,handler in [('SqlContracts.cs','SqlContracts')]+[(f.name,f.stem) for f in sorted(p.glob('*Cases.cs'))]:
 if not (p/filename).exists():continue
 text=(p/filename).read_text(encoding='utf-8')
 for c in d['cases']:
  if '"'+c['key']+'"' in text:handlers[c['key']]=handler
out=['using System.Text.Json;using Xunit;using Xunit.Abstractions;using Xunit.Sdk;',
 '[TestCaseOrderer("BottomUpOrder", "IntegrationCurrent")] public sealed class IntegrationTests {']
for c in d['cases']:
 skip='' if c['key'] in handlers else ',Skip="Chưa triển khai script cho TC này; không tính là đã thực thi"'
 out.append(f'[Fact(DisplayName="{c["id"]} | {c["title"]}"{skip})][Trait("TestCaseId","{c["id"]}")][Trait("Group","{c["group"]:02}")][Trait("Stage","BU{c["stage"]}")] public async Task {c["id"].replace("-","_")}() => await Run("{c["id"]}","{c["key"]}");')
out+=['static async Task Run(string id,string key){await using var f=new Fixture(id);var doc=JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory,"Manifest.json")));var c=doc.RootElement.GetProperty("cases").EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);f.Proof["baseline"]=c.Clone();try {await f.Init();switch(key){']
for handler in sorted(set(handlers.values())):
 for key,h in handlers.items():
  if h==handler:out.append(f'case "{key}":')
 out.append(f'await Cases.{handler}(f,key);break;')
out+=['default:throw new InvalidOperationException("No implementation: "+id); } f.Proof["assertions"]="Passed";}catch(Exception ex){f.Proof["exception"]=ex.ToString();throw;}}}',
'''public sealed class BottomUpOrder:ITestCaseOrderer {
 public IEnumerable<T> OrderTestCases<T>(IEnumerable<T> cases) where T:ITestCase {
 var doc=JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"Manifest.json")));
 var ranks=doc.RootElement.GetProperty("cases").EnumerateArray().ToDictionary(c=>c.GetProperty("id").GetString()!.Replace('-','_'),c=>c.GetProperty("executionOrder").GetInt32());
 return cases.OrderBy(c=>ranks.GetValueOrDefault(c.TestMethod.Method.Name,int.MaxValue));}}
''']
(p/'IntegrationTests.cs').write_text('\n'.join(out),encoding='utf-8')
(p/'implementation.json').write_text(json.dumps({'implemented':[c['id'] for c in d['cases'] if c['key'] in handlers],'pending':[c['id'] for c in d['cases'] if c['key'] not in handlers]},indent=2))
print('Cases',len(d['cases']),'Implemented',len(handlers),'Pending',len(d['cases'])-len(handlers))
