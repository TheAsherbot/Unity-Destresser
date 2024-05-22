using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using UnityEngine;

namespace TheAshBot.Assets.Destresser
{
    public class DestresserImageGenerator
    {

        private const string API_KEY = "rXXiPYqMi694dU2Jr2r5t563d6U42TfJitRLeIv4eeivYVGgBu5gm20q";

        private const int NUMBER_OF_PICTURES = 8000;





        public async static Task<Response> GetCatPicture()
        {
            Request request = new Request();
            request.query = "Cute Cats";

            SearchResults searchResults = await GetRandomPicture(request);

            Texture2D texture = await GetPictureFromUrl(searchResults.photos[0]);

            Response response = new Response();
            response.picture = texture;
            response.photographer = searchResults.photos[0].photographer;
            response.url = searchResults.photos[0].url;
            response.width = searchResults.photos[0].width;
            response.height = searchResults.photos[0].height;

            return response;
        }

        private async static Task<SearchResults> GetRandomPicture(Request request)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", API_KEY);
            string url = $"https://api.pexels.com/v1/search?query={request.query}&page={UnityEngine.Random.Range(0, NUMBER_OF_PICTURES)}&per_page=1";

            HttpResponseMessage httpResponse = await client.GetAsync(url);

            if (!httpResponse.IsSuccessStatusCode)
            {
                // Error
                Debug.Log("CAT GENERATION ERROR. please contact me about this error by going to theasherbot.com and sending me an email. thank you.\n\n"
                    + await httpResponse.Content.ReadAsStringAsync() + "\n\n" + httpResponse.StatusCode);
                return default(SearchResults);
            }

            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            SearchResults response = JsonUtility.FromJson<SearchResults>(responseContent);

            return response;
        }

        private async static Task<Texture2D> GetPictureFromUrl(SearchResults.Photo photo)
        {
            Texture2D result = new Texture2D(photo.width, photo.height);
            HttpClient client = new HttpClient();

            string url = $"https://images.pexels.com/photos/{photo.id}/pexels-photo-{photo.id}.jpeg";

            HttpResponseMessage httpResponse = await client.GetAsync(url);

            byte[] responseContent = await httpResponse.Content.ReadAsByteArrayAsync();

            if (!ImageConversion.LoadImage(result, responseContent, false))
            {
                Debug.Log("CAT GENERATION ERROR. please contact me about this error by going to theasherbot.com and sending me an email. thank you.\n\n"
                    + await httpResponse.Content.ReadAsStringAsync() + "\n\n" + httpResponse.StatusCode);
            }

            return result;
        }



        [Serializable]
        private struct Request
        {
            public string query;
            public string orientation;
            public string size;
            public string color;
            public string locale;
            public int page;
            public int per_page;
        }
        [Serializable]
        private struct SearchResults
        {

            [Serializable]
            public struct Photo
            {

                [Serializable]
                public struct Src
                {
                    public string original;
                    public string large2x;
                    public string large;
                    public string medium;
                    public string small;
                    public string portrait;
                    public string landscape;
                    public string tiny;
                }


                public int id;
                public int width;
                public int height;
                public string url;
                public string photographer;
                public string photographer_url;
                public int photographer_id;
                public string avg_color;
                public Src src;
                public bool liked;
                public string alt;
            }


            public int total_results;
            public int page;
            public int per_page;
            public Photo[] photos;
            public string next_page;
        }

        [Serializable]
        public struct Response
        {
            public int width;
            public int height;
            public string photographer;
            public string url;
            public Texture2D picture;
        }

    }
}