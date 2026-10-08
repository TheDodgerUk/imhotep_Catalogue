import sys
import json
from jsonmerge import merge

def load_json(file_path):
    with open(file_path) as f:
        return json.load(f)

def get_schema(type):
    if (type == 'list'):
        return {
            "mergeStrategy": "arrayMergeById",
            "mergeOptions": { "idRef": '/' }
        }
    if (type == 'dict'):
        return {
            "mergeStrategy": "objectMerge",
            "mergeOptions": { "objClass": "OrderedDict" }
        }

current = load_json(sys.argv[1])
other = load_json(sys.argv[2])

result = merge(current, other, get_schema(sys.argv[4]))

with open(sys.argv[3], 'w') as f:
    json.dump(result, f, indent=2)