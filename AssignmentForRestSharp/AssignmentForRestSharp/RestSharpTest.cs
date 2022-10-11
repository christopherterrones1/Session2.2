using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace AssignmentForRestSharp
{
    [TestClass]
    public class Session2RestSharp
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string UserEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {
            #region CreateUser
            //Create User

            Random rnd = new Random();
            int randomId = rnd.Next(10000000, 99999999);
            int randomPetId = rnd.Next(1, 99);
            int randomPetTagId = rnd.Next(1, 99);

            Category category = new Category();
            category.Id = randomPetId;
            category.Name = "Dog";

            Category tag = new Category();
            tag.Id = randomPetTagId;
            tag.Name = "Siberian Husky";

            List<string> photoUrls = new List<string>();
            photoUrls.Add("www.google.com");

            List<Category> tags = new List<Category>();
            tags.Add(tag);

            var newPet = new PetModel()
            {
                Id = randomId,
                Category = category,
                Name = "Saber",
                PhotoUrls = photoUrls,
                Tags = tags,
                Status = "Available"
            };

            // Send Post Request
            var temp = GetURI(UserEndpoint);
            var postRestRequest = new RestRequest(GetURI(UserEndpoint)).AddJsonBody(newPet);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);

            //Verify POST request status code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200");
            #endregion

            #region GetUser
            var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{newPet.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetModel>(restRequest);
            #endregion

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(newPet.Name, restResponse.Data.Name, "Pet Name did not match.");
            Assert.AreEqual(newPet.Category.Id, restResponse.Data.Category.Id, "Pet Category ID did not match.");
            Assert.AreEqual(newPet.Category.Name, restResponse.Data.Category.Name, "Pet Category ID did not match.");
            Assert.AreEqual(newPet.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "PhotoURLs did not match.");
            Assert.AreEqual(newPet.Tags[0].Id, restResponse.Data.Tags[0].Id, "Pet Tag ID did not match.");
            Assert.AreEqual(newPet.Tags[0].Name, restResponse.Data.Tags[0].Name, "Pet Tag ID did not match.");
            #endregion

            #region CleanUp
            cleanUpList.Add(newPet);
            #endregion
        }
    }

}
