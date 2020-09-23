using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Service.Process {
    class GitHub {

        private static readonly HttpClient client = new HttpClient();
        private readonly DirectoryInfo _directory;

        public List<Repository> Repositoryes { get; set; }

        public GitHub() {
            _directory = DirectoryProcess.CreateDirectory(this.GetType().Name);
            Repositoryes = new List<Repository>();
        }

        public async Task InitBackup() {
            await ListOrganizationRepositories();

            foreach (var repos in Repositoryes) {
                CloneProcessAsync(repos);
            }
        }

        private bool CloneProcessAsync(Repository repos) {
            try {
                var process = new System.Diagnostics.Process();
                process.StartInfo.WorkingDirectory = _directory.FullName;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = "git";
                process.StartInfo.Arguments = $"clone {repos.Url}";

                process.Start();
                process.WaitForExit();

                //CheckoutProcessAsync(repos);

                Console.WriteLine($"clone {repos.Url} successfully.");
                return true;
            } catch (Exception) {
                return false;
            }
        }

        private void CheckoutProcessAsync(Repository repos) {
            foreach (var branche in repos.Branches) {
                if (branche.Name != "master") {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.WorkingDirectory = Path.Combine(_directory.FullName, repos.Name);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.FileName = "git";
                    process.StartInfo.Arguments = $"checkout {branche.Name}";

                    process.Start();
                    process.WaitForExit();
                    Console.WriteLine($"checkout {branche.Name} successfully.");
                }
            }
        }

        private async Task ListOrganizationRepositories() {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "KEY");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Way Data Solution");

            Task<Stream> stream = client.GetStreamAsync($"https://api.github.com/orgs/wayds/repos");

            List<Repository> repositories = await JsonSerializer.DeserializeAsync<List<Repository>>(await stream);

            foreach (var repos in repositories) {
                repos.Branches = await Listbranches(repos);
            }

            this.Repositoryes = repositories;

            Console.WriteLine($"Get repositorys successfully.");
        }

        private async Task<List<Branche>> Listbranches(Repository repository) {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "KEY");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Way Data Solution");

            Task<Stream> stream = client.GetStreamAsync($"https://api.github.com/repos/wayds/{repository.Name}/branches");

            List<Branche> branches = await JsonSerializer.DeserializeAsync<List<Branche>>(await stream);

            Console.WriteLine($"Repository {repository.Name} get branchs successfully.");

            return branches;
        }
    }

    class Repository {

        [JsonPropertyName("html_url")]
        public Uri Url { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        public List<Branche> Branches { get; set; }

        public bool IsClone { get; set; }

        public int AmountTentative { get; set; }

        public Repository() {
            Branches = new List<Branche>();
        }
    }

    class Branche {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public Branche() { }
    }
}
