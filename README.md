# SendGridCore
SendGridCore is a C# Web API for creating SendGrid email contacts and adding them to a specific marketing list.

It is implemented using Dotnet Core version 1.0

It acts as a C# wrapper for the SendGrid V3 REST API and is easily extensible. 

It was originally written to be used in conjunction with an anonymous signup to an email list.

The REST call takes three parameters; email, firstname and lastname e.g.

`http://localhost:62209/api/ListManage/?email=jane@example.com&firstname=Jane&lastname=Jones`

This will add `jane@example.com` to the SendGrid contacts if not already present. If already present it will update first name and last name.

It will then add Jane's contact Id to the list configured in the configuration ListToManage

An obvious extension to the code would be to add a method to add Jane into a database as a new member of the list and trigger a welcome email or complete drip campaign.


