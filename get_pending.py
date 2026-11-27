import json
p='tests/IntegrationCurrent/Manifest.json';d=json.load(open(p,encoding='utf-8-sig'))
for id in ['IT-130','IT-176','IT-177','IT-178','IT-179','IT-180','IT-181','IT-187']:
 c=next(x for x in d['cases'] if x['id']==id);print('\n###',id,c['key'],c['title']);print(c['input']);print(c['steps']);print('EXPECTED:',c['expected'])
