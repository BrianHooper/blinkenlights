import os

def IsNullOrEmpty(data):
    return data is None or len(data) == 0

def ReadTsv(filename):
    if IsNullOrEmpty(filename) or not os.path.exists(filename):
        return []
    with open(filename, "r", encoding="utf-8") as infile:
        data = infile.readlines()
    if IsNullOrEmpty(data):
        return []
    
    data = [x.replace("\n", "").replace("\r", "") for x in data if len(x) > 0]
    data = [x.split("\t") for x in data]
    return data

def WriteTsv(filename, data):
    if IsNullOrEmpty(filename) or IsNullOrEmpty(data):
        return False

    output_strings = ["\t".join([str(y) for y in x]) + "\n" for x in data]
    with open(filename, "w", encoding="utf-8") as outfile:
        outfile.writelines(output_strings)
    return True