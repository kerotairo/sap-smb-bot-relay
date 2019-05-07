from flask import Flask, request, jsonify
import requests
import json
import os
api_url = os.environ["smb_cai_api"]

app = Flask(__name__)

@app.route('/', methods=["GET"])
def index():
    return jsonify(status=200)

@app.route('/sales-analysis',methods = ["POST"])
def sales_analysis():
    body = request.get_json()
    print(body)
    if body["conversation"]["memory"]["count"]["raw"]:
        count = "count=" + str(body["conversation"]["memory"]["count"]["raw"])
    else:
        count = ""
    if body["conversation"]["memory"]["sort_direction"]["raw"]:
        sort_raw = str(body["conversation"]["memory"]["sort_direction"]["raw"])
        print(sort_raw)
        sort = "sort=desc" if sort_raw in ["best","most","top"] else "sort=asc"
    else:
        sort_raw = ""
    
    if "description" in body["conversation"]["memory"]:
        description = str(body["conversation"]["memory"]["description"]["raw"])
        print(description)
        if description in ["any","all"]:
            desc = ""
        else:
            desc = "descr=" + description
    else:
        desc = ""

    
    if "dimension" in body["conversation"]["memory"]:
        dimension = body["conversation"]["memory"]["dimension"]["raw"]
        needs_description = body["conversation"]["memory"]["dimension"]["needs-description"]
 
    if dimension in ["opportunity", "opportunities"]:
        node = "GetOpportunities"
    elif dimension in ["customers", "customer", "client","clients","business partners"]:
        node = "GetCustomers"
    elif dimension in ["items","item","sales item","sales","products","product","best-sellers","best sellers"]:
        node= "GetProducts"

    url = api_url+node +"?" + count +"&" + sort + '&' + desc
    print(url)
    result = requests.request("GET", url)
    return result.content

if __name__ == '__main__':
    app.run()