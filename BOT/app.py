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
            return ""
        else:
            return description
    else:
        return ""

def get_sort(body):
    if "sort_direction" in body["conversation"]["memory"]:
        sort_raw = str(body["conversation"]["memory"]["sort_direction"]["raw"])
        print(sort_raw)
        sort = "desc" if sort_raw in ["best","most","top"] else "asc"
        return sort
    else:
        return ""

def get_count(body):
    if "count" in body["conversation"]["memory"]:
        return str(body["conversation"]["memory"]["count"]["raw"])
    else:
        return ""

@app.route('/', methods=["GET"])
def index():
    return jsonify(status=200)

@app.route('/sales-analysis',methods = ["POST"])
def sales_analysis():
    body = request.get_json()

    print(body)
    params = {
        "count": get_count(body),
        "sort" : get_sort(body),
        "descr" : get_desription(body)
    }
    print(params)
    url = api_url + get_node(get_dimension(body))
    
    print(url)
    result = requests.request("GET", url, params=params)
    return result.content

if __name__ == '__main__':
    app.run()