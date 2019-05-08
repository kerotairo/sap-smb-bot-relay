from flask import Flask, request, jsonify
import requests
import json
import os
api_url = os.environ["smb_cai_api"]

app = Flask(__name__)

nodes = {
    "GetOpportunities": ["opportunity","opportunities"],
    "GetCustomers": ["customers", "customer", "client","clients","business partners"],
    "GetProducts": ["items","item","sales item","sales","products","product","best-sellers","best sellers"]
}

def get_node(dimension):
    for key in nodes:
        if dimension in nodes[key]:
            return key

def get_dimension(body):
    if "dimension" in body["conversation"]["memory"]:
        dimension = body["conversation"]["memory"]["dimension"]["raw"]
        needs_description = body["conversation"]["memory"]["dimension"]["needs-description"]
        return dimension
    else:
        return ""

def get_desription(body):
    if "description" in body["conversation"]["memory"]:
        description = str(body["conversation"]["memory"]["description"]["raw"])
        print(description)
        if description in ["any","all"]:
            desc = ""
        else:
            desc = "descr=" + description
    else:
        desc = ""
    return desc

def get_sort(body):
    if body["conversation"]["memory"]["sort_direction"]["raw"]:
        sort_raw = str(body["conversation"]["memory"]["sort_direction"]["raw"])
        print(sort_raw)
        sort = "sort=desc" if sort_raw in ["best","most","top"] else "sort=asc"
    else:
        sort= ""
    return sort

def get_count(body):
    if body["conversation"]["memory"]["count"]["raw"]:
        count = "count=" + str(body["conversation"]["memory"]["count"]["raw"])
    else:
        count = ""
    return count

@app.route('/', methods=["GET"])
def index():
    return jsonify(status=200)

@app.route('/sales-analysis',methods = ["POST"])
def sales_analysis():
    body = request.get_json()

    print(body)

    count = get_count(body)
    sort = get_sort(body)
    desc = get_desription(body)
    dimension = get_dimension(body)

    print(count,sort,desc, dimension)
    url = api_url + get_node(dimension) +"?" + count +"&" + sort + '&' + desc
    
    print(url)

    result = requests.request("GET", url)
    return result.content

if __name__ == '__main__':
    app.run()