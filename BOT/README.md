# SMB Bot Lambda Relay
This app is deployed as an AWS Lambda function to map the bot's memory to the backend API. 

# Installation
This app was launched through Zappa and Flask
* Create and activate virtualenv in the parent directory
* Install packages in requirements.txt
* Exececute `zappa init` to create zappa_settings.json
* Execute `zappa deploy` to deploy the function into AWS Lambda
