import json
import geopandas as gpd
import pandas as pd
import sqlite3
import numpy as np
from typing import List, Dict, Any, Optional

# classes
class Dataset:
    def __init__(self, id, path, fields, storage_type, data_type, tablename=None):
        self.id = id
        self.path = path
        self.storage_type = storage_type.lower() # Convert to lowercase here
        self.data_type = data_type.lower()
        self.fields = {f['fieldtype']: {'fieldname': f['fieldname'], 'selection': f.get('selection', [])} for f in fields}
        self.tablename = tablename

    def read(self):
        try:
            if self.storage_type == 'shapefile':
                self.data = gpd.read_file(self.path)
            elif self.storage_type == 'sqlite':
                connection = sqlite3.connect(self.path)
                query = f"SELECT * FROM {self.tablename}"
                self.data = pd.read_sql_query(query, connection)
                connection.close()
        except FileNotFoundError:
                print(f"The file {self.path} does not exist.")
        except PermissionError:
                print(f"Permission denied to read the file {self.path}.")

        # Set index if 'featureidx' field exists
        featureidx_field = [f for f in self.fields.values() if f['fieldname'] == 'FEATUREIDX']
        if featureidx_field:
            self.data.set_index(featureidx_field[0]['fieldname'], inplace=True)


    def write(self):
        if self.storage_type == "shapefile":
            self.data.to_file(self.path)

class ClassificationClass:
    def __init__(self, lbound, ubound, penalty, result_text):
        self.lbound = lbound
        self.ubound = ubound
        self.penalty = penalty
        self.result_text = result_text

class Classification:
    def __init__(self, id, classes):
        self.id = id
        self.classes = [ClassificationClass(cls['lbound'], cls['ubound'], cls['penalty'], cls['result_text']) for cls in classes]

class Subset:
    def __init__(self, fieldtype: str, values: List[str]):
        self.fieldtype = fieldtype
        self.values = values


class Filter:
    def __init__(self,field_type,evaluation, value):
        self.field_type = field_type
        self.evaluation = evaluation
        self.value = value

class ResultFieldNames:
    def __init__(self, text_field, penalty_field):
        self.text_field = text_field
        self.penalty_field = penalty_field

class RuleExecution:
    def __init__(self, method: str, field_type: Optional[str] = None, classification_id: Optional[str] = None, value: Optional[float] = None):
        self.method = method
        self.field_type = field_type
        self.classification_id = classification_id
        self.value = value

class Rule:
    def __init__(self, order: int, execute: bool, name: str, input_dataset: str, origin: str,
                 scenarios: List[str], subset: Optional[List[Dict[str, Any]]] = None,
                 filter: Optional[Dict[str, Any]] = None, input_field: Optional[str] = None,
                 rule_execution: Optional[Dict[str, Any]] = None):
        self.order = order
        self.execute = execute
        self.name = name
        self.input_dataset = input_dataset
        self.origin = origin
        self.scenarios = scenarios
        self.subset = [Subset(**s) for s in subset] if subset else None
        self.filter = Filter(**filter) if filter else None
        self.input_field = input_field
        self.rule_execution = RuleExecution(**rule_execution) if rule_execution else None

# the function for determining the results field name for a given scenario, rule and results type!
def get_results_field_name(scenario, rule, results_type):
    if isinstance(rule.order, int):
        order = str(rule.order)
    else:
        order = rule.order

    if results_type == "TXT":
        print(f"Setting results field to ", scenario + 'TXT' + order)
        return scenario + 'TXT' + order
    elif results_type == "PEN":
        print(f"Setting results field to ", scenario + 'PEN' + order)
        return scenario + 'PEN' + order
    else:
        print(f"Invalid results_type {results_type}. Expected 'TXT' or 'PEN'.")
        return None

# the function for processing rules!
def process_rules(rules, datasets, classifications, results_dataset):

    # Iterate over scenarios
    for scenario in scenarios:

        print(f"Processing scenario {scenario}...")
 
        for rule in rules:
            if not rule.execute:
                continue

            print(f"Processing rule {rule.name}...")

            # Get input dataset
            input_dataset = datasets.get(rule.input_dataset, None)
            if input_dataset is None:
                print(f"Input dataset {rule.input_dataset} not found.")
                continue

            # Read the input dataset and set the fieldname containing the datavalue we'll be processing
            input_dataset.read()

            # Only fetch field_type when method is not 'constant'
            if rule.rule_execution.method != "constant":
                datavalue_fieldname = input_dataset.fields[rule.rule_execution.field_type]['fieldname']

            # Read the output dataset
            results_dataset.read()

            # Check if the current scenario is also present in the current rule's scenarios list
            if scenario not in rule.scenarios:
                continue
             
            # Apply subset filters to input dataset
            subset_data = input_dataset.data
            if rule.subset:
                for subset in rule.subset:
                    fieldtype = subset.fieldtype
                    values = subset.values
                    if fieldtype in input_dataset.fields:
                        fieldname = input_dataset.fields[fieldtype]['fieldname']
                        subset_data = subset_data[subset_data[fieldname].isin(values)]

            # add scenario to subset filters
            if 'scenario' in input_dataset.fields:
                fieldname = input_dataset.fields['scenario']['fieldname']
                subset_data = subset_data[subset_data[fieldname] == scenario]

            # Apply filter criteria
            if rule.filter and rule.filter.field_type in input_dataset.fields:
                fieldname = input_dataset.fields[rule.filter.field_type]['fieldname']
                if rule.filter.evaluation == "=":
                    subset_data = subset_data[subset_data[fieldname] == rule.filter.value]
                # add other evaluation conditions here (>, <, !=, etc.)

            # Initialize 'penalty' and 'result_text' columns for this rule with default values
            subset_data[get_results_field_name(scenario, rule, "PEN")] = np.nan
            subset_data[get_results_field_name(scenario, rule, "TXT")] = None

            if rule.rule_execution.method == "classification":
                classification = classifications.get(rule.rule_execution.classification_id, None)
                if classification is None:
                    print(f"Classification {rule.rule_execution.classification_id} not found.")
                    continue

                # Assuming the classification 'classes' list is sorted in ascending order by 'lbound'
                for cls in classification.classes:
                    print(f"Class bounds: {cls.lbound} to {cls.ubound}, penalty: {cls.penalty}, result text: {cls.result_text}")
                    condition = (subset_data[datavalue_fieldname] >= cls.lbound) & (subset_data[datavalue_fieldname] < cls.ubound)
                    subset_data.loc[condition, get_results_field_name(scenario, rule, "PEN")] = cls.penalty
                    subset_data.loc[condition, get_results_field_name(scenario, rule, "TXT")] = cls.result_text

            elif rule.rule_execution.method == "constant":
                # we apply a constant penalty to all records that pass the filter
                subset_data.loc[:, get_results_field_name(scenario, rule, "PEN")] = rule.rule_execution.value
                subset_data.loc[:, get_results_field_name(scenario, rule, "TXT")] = rule.name

            # Now that we have the penalties and text results, we can append this data back to the original dataset
            # Update only the fields related to the current rule and scenario in the output dataset
            for field in [get_results_field_name(scenario, rule, "PEN"), get_results_field_name(scenario, rule, "TXT")]:
                results_dataset.data.loc[subset_data.index, field] = subset_data[field]

            results_dataset.data[scenario] -= subset_data[get_results_field_name(scenario, rule, "PEN")]  # update the rating

            # Write the updated output dataset
            results_dataset.write()

# global variables & script
json_file_path = r"c:\GITHUB\klimaatatlas\rekenregels\20230530_rekenregels.json"

scenarios = []                  # create a list to store all scenarios
datasets = {}                   # Create a dictionary to store all datasets
classifications = {}            # Create a dictionary to store all classifications
rules = []                      # Create a list to store all rules

# read the JSON file, which contains the datasets and the rules to assess
print(f"reading json file")
with open(json_file_path) as json_file:
    schema = json.load(json_file)

# read scenarios from the schema
scenarios = schema['scenarios']    

# populate the datasets
print(f"populating the datasets")
for dataset in schema['datasets']:
    tablename = dataset.get('tablename', None)  # Get the 'tablename' if it exists
    fields = dataset['fields']
    storage_type = dataset['storage_type']
    data_type = dataset['data_type']
    path = dataset['path']
    id = dataset['id']

    # Instantiate Dataset
    datasets[dataset['id']] = Dataset(id, path, fields, storage_type, data_type, tablename)

# populate the classifications
print(f"populating the classification schemas")
for classification in schema['classifications']:
    classifications[classification['id']] = Classification(classification['id'], classification['classes'])

# populate the validation rules to be assessed
print(f"populating the list of rules")
for rule in schema['rules']:
    rules.append(Rule(**rule))

# read results_dataset from the schema
print(f"initializing the results dataset")
results_dataset = None  # Create a variable to store the results dataset
results_dataset_id = schema['results']['dataset']
results_dataset = datasets[results_dataset_id]
results_dataset.read()  # read the results_dataset

# Create a field for the results in the results_dataset for each scenario and initialize its rating to 10
for scenario in scenarios:
    results_dataset.data[scenario] = 10

# save the initialized results dataset
results_dataset.write()

# now start processing the rules!
print(f"processing the rules")
process_rules(rules, datasets, classifications, results_dataset)

