This is a windows service which uses dapper and restsharp to get all data from orders of a specified client and send it to a shipping company.
First, the application gets the integrated clients and take all necessary data from them. Then, the application will run a query and take all required orders from that client.
After do that, the service will create a token serializing a json with required informations from the client as user, domain and password.
Once the token is created, it will send a request to an api which has to validate if the data is true.
After the token creation, the application will take all orders and build a json with the collected data from each order.
Once the json's from all orders are built, the service will send a request to the shipping company api sending all orders.
The api will return success if all the required fields was succesfully sent by the service.
