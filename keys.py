import json
for c in json.load(open('tests/IntegrationCurrent/Manifest.json',encoding='utf-8-sig'))['cases']:
 if c['id'] in ['IT-012','IT-027','IT-031','IT-032','IT-033','IT-081','IT-086','IT-089','IT-099','IT-101','IT-125','IT-168']:
  print(c['id'],c['key'])
