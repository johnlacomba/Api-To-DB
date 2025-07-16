namespace API_To_DB
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Amazon.Runtime;
    using Amazon.Runtime.CredentialManagement;
    using Amazon.SecurityToken;

    public class CallAPIAndUpload
    {
        // Configure the API endpoint and API key
        private const string URL = "https://fake-json-api.mock.beeceptor.com/companies";
        private const string UrlParameters = "?api_key=123";

        // Configure the AWS account credentials using Visual Studio's AWS Explorer
        private static AWSCredentials GetAccountCredentials(string profileName = "Esparno")
        {
            var chain = new CredentialProfileStoreChain();
            if (chain.TryGetAWSCredentials(profileName, out AWSCredentials awsCredentials))
            {
                return awsCredentials;
            }
            else
            {
                Console.WriteLine("{0}", "ERROR - There was a problem getting the AWS credentials.");
                throw new ArgumentException("ERROR - There was a problem getting the AWS credentials.");
            }
        }

        private static void Main(string[] args)
        {
            // Create the DynamoDB client object
            var ddbClient = new AmazonDynamoDBClient(GetAccountCredentials(), Amazon.RegionEndpoint.USEast1);
            #pragma warning disable CS0618 // Type or member is obsolete

            // Create the DynamoDB table object
            var table = Table.LoadTable(ddbClient, "API-To-DB");
            #pragma warning restore CS0618 // Type or member is obsolete

            // Create the Http client object
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // Call the API and check if the response shows success
            HttpResponseMessage response = client.GetAsync(UrlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs
            if (response.IsSuccessStatusCode)
            {
                // Parse the http response body
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                foreach (var d in dataObjects)
                {
                    Console.WriteLine("{0}", d.Name + " | " + d.Address + " | " + d.Zip + " | " + d.Country + " | " + d.EmployeeCount + " | " + d.Industry + " | " + d.MarketCap + " | " + d.Domain + " | " + d.Logo + " | " + d.CeoName );

                    // Create the request for putItem
                    var putItemRequest = new PutItemRequest
                    {
                        TableName = "API-To-DB",
                        Item = new Dictionary<string, AttributeValue> { },
                    };

                    try
                    {
                        // Add attributes from the API response to the PutItemRequest object
                        putItemRequest.Item.Add("Country", new AttributeValue
                        {
                            S = d.Country,
                        });

                        putItemRequest.Item.Add("Name", new AttributeValue
                        {
                            S = d.Name,
                        });

                        putItemRequest.Item.Add("Address", new AttributeValue
                        {
                            S = d.Address,
                        });

                        putItemRequest.Item.Add("Zip", new AttributeValue
                        {
                            S = d.Zip,
                        });

                        putItemRequest.Item.Add("EmployeeCount", new AttributeValue
                        {
                            S = d.EmployeeCount,
                        });

                        putItemRequest.Item.Add("Industry", new AttributeValue
                        {
                            S = d.Industry,
                        });

                        putItemRequest.Item.Add("MarketCap", new AttributeValue
                        {
                            S = d.MarketCap,
                        });

                        putItemRequest.Item.Add("Domain", new AttributeValue
                        {
                            S = d.Domain,
                        });

                        putItemRequest.Item.Add("Logo", new AttributeValue
                        {
                            S = d.Logo,
                        });

                        putItemRequest.Item.Add("CeoName", new AttributeValue
                        {
                            S = d.CeoName,
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0}", ex);
                        throw;
                    }

                    // Upload the items to the DB and return the status code
                    var putItemResponse = ddbClient.PutItemAsync(putItemRequest).Result;
                    Console.WriteLine("{0}", putItemResponse.HttpStatusCode);
                }
            }
            else
            {
                // If the http request isn't successful return the status code
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous
            client.Dispose();
        }
    }
}