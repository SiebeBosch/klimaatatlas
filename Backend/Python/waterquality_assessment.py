import json
import geopandas as gpd
import pandas as pd
import sqlite3
import numpy as np

###################################################################################################################
# classes
###################################################################################################################
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
    def __init__(self,method, id):
        self.method = method
        self.id = id

class Rule:
    def __init__(self, order, execute, name, input_dataset, subset, filter, rule_execution, output_dataset, origin, result_fieldnames, scenarios):
        self.order = order
        self.execute = execute
        self.name = name
        self.input_dataset = input_dataset
        self.subset = {s['fieldtype']: s['values'] for s in subset}
        self.filter = Filter(**filter)  # Filter instance
        self.rule_execution = RuleExecution(**rule_execution)  # Rule instance
        self.output_dataset = output_dataset
        self.origin = origin
        # Instead of creating a ResultFieldNames instance, create a dictionary where keys are scenarios and values are dictionaries of field names
        self.result_fieldnames = {scenario: {k: v + scenario for k, v in result_fieldnames.items()} for scenario in scenarios}
  
###################################################################################################################


###################################################################################################################
# the function for processing rules!
###################################################################################################################
def process_rules(rules, datasets, classifications):
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
        datavalue_fieldname = input_dataset.fields['datavalue']['fieldname']

        # Get output dataset
        output_dataset = datasets.get(rule.output_dataset, None)
        if output_dataset is None:
            print(f"Output dataset {rule.output_dataset} not found.")
            continue

        # Read the output dataset
        output_dataset.read()

        # Iterate over scenarios
        for scenario in input_dataset.fields['scenario']['selection']:
            
            # Apply subset filters to input dataset
            subset_data = input_dataset.data

            for fieldtype, values in rule.subset.items():
                if fieldtype in input_dataset.fields:
                    fieldname = input_dataset.fields[fieldtype]['fieldname']
                    subset_data = subset_data[subset_data[fieldname].isin(values)]

            # add scenario to subset filters
            if 'scenario' in input_dataset.fields:
                fieldname = input_dataset.fields['scenario']['fieldname']
                subset_data = subset_data[subset_data[fieldname] == scenario]
                
            # Apply filter criteria
            if rule.filter.field_type in input_dataset.fields:
                fieldname = input_dataset.fields[rule.filter.field_type]['fieldname']
                if rule.filter.evaluation == "=":
                    subset_data = subset_data[subset_data[fieldname] == rule.filter.value]
                # add other evaluation conditions here (>, <, !=, etc.)

            print(f"Subset is ", subset_data)

            # Initialize 'penalty' and 'result_text' columns for this rule with default values
            subset_data[rule.result_fieldnames[scenario]['penalty_field']] = np.nan
            subset_data[rule.result_fieldnames[scenario]['text_field']] = None

            # Apply classification method and calculate penalty
            if rule.rule_execution.method == "classification":
                classification = classifications.get(rule.rule_execution.id, None)
                if classification is None:
                    print(f"Classification {rule.rule_execution.id} not found.")
                    continue

            # Assuming the classification 'classes' list is sorted in ascending order by 'lbound'
            for cls in classification.classes:
                print(f"Class bounds: {cls.lbound} to {cls.ubound}, penalty: {cls.penalty}, result text: {cls.result_text}")
                print(f"fieldname ", fieldname)
                print(f"value ", subset_data[datavalue_fieldname])
                condition = (subset_data[datavalue_fieldname] >= cls.lbound) & (subset_data[datavalue_fieldname] < cls.ubound)
                subset_data.loc[condition, rule.result_fieldnames[scenario]['penalty_field']] = cls.penalty
                subset_data.loc[condition, rule.result_fieldnames[scenario]['text_field']] = cls.result_text


            #print(f"Subset after implementation of rule is ", subset_data)


            #exit()

            # Now that we have the penalties and text results, we can append this data back to the original dataset
            # Update only the fields related to the current rule and scenario in the output dataset
            for field in [rule.result_fieldnames[scenario]['penalty_field'], rule.result_fieldnames[scenario]['text_field']]:
                output_dataset.data.loc[subset_data.index, field] = subset_data[field]

            # Write the updated output dataset
            output_dataset.write()

###################################################################################################################



###################################################################################################################
# global variables & script
###################################################################################################################
json_file_path = r"c:\GITHUB\klimaatatlas\rekenregels\20230530_rekenregels.json"

datasets = {}                   # Create a dictionary to store all datasets
classifications = {}            # Create a dictionary to store all classifications
rules = []                      # Create a list to store all rules

# read the JSON file, which contains the datasets and the rules to assess
with open(json_file_path) as json_file:
    schema = json.load(json_file)

# populate the datasets
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
for classification in schema['classifications']:
    classifications[classification['id']] = Classification(classification['id'], classification['classes'])

# populate the validation rules to be assessed
for rule in schema['rules']:
    rules.append(Rule(**rule))

# now start processing the rules!
process_rules(rules, datasets, classifications)

###################################################################################################################
