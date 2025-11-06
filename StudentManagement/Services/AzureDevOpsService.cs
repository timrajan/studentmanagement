using StudentManagement.Models;
using System.Text.Json;

namespace StudentManagement.Services
{
    public class AzureDevOpsService
    {
        // TODO: Replace these with your actual Azure DevOps configuration
        private readonly string _organization = "YourOrganization";  // e.g., "contoso"
        private readonly string _project = "YourProject";            // e.g., "MyProject"
        private readonly int _pipelineId = 123;                      // Your pipeline ID number
        private readonly string _pat = "YOUR_PERSONAL_ACCESS_TOKEN"; // Generate from Azure DevOps
        private readonly string _branch = "refs/heads/main";         // or "refs/heads/master"

        /// <summary>
        /// Triggers an Azure DevOps build pipeline with the provided study record data
        /// </summary>
        /// <param name="record">The study record containing form data</param>
        /// <returns>Tuple with success status and message</returns>
        public (bool Success, string Message) TriggerBuildPipeline(StudyRecord record)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Azure DevOps REST API endpoint to trigger pipeline
                    var url = $"https://dev.azure.com/{_organization}/{_project}/_apis/pipelines/{_pipelineId}/runs?api-version=7.0";

                    // Add Basic Authentication with PAT
                    var authToken = Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes($":{_pat}"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                    // Build the payload with pipeline parameters
                    // These parameter names must match your Azure pipeline YAML parameter names
                    var payload = new
                    {
                        resources = new
                        {
                            repositories = new
                            {
                                self = new
                                {
                                    refName = _branch
                                }
                            }
                        },
                        templateParameters = new
                        {
                            // IMPORTANT: Pass the record ID so webhook can update the correct record
                            recordId = record.Id.ToString(),
                            // Map form fields to pipeline parameters
                            team = record.Team ?? "",
                            firstName = record.FirstName ?? "",
                            middleName = record.MiddleName ?? "",
                            lastName = record.LastName ?? "",
                            dateOfBirth = record.DateOfBirth ?? "",
                            emailAddress = record.EmailAddress ?? "",
                            studentIdentityID = record.StudentIdentityID ?? "",
                            studentInitialID = record.StudentInitialID ?? "",
                            environment = record.Environment ?? "",
                            studentIQLevel = record.StudentIQLevel ?? "",
                            studentRollNumber = record.StudentRollNumber ?? "",
                            studentRollName = record.StudentRollName ?? "",
                            studentParentEmailAddress = record.StudentParentEmailAddress ?? "",
                            status = record.Status ?? "",
                            type = record.Type ?? "",
                            tags = record.Tags ?? "",
                            release = record.Release ?? ""
                        }
                    };

                    // Serialize to JSON
                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    // Send POST request to Azure DevOps
                    var response = httpClient.PostAsync(url, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;
                        return (true, "Azure DevOps pipeline triggered successfully!");
                    }
                    else
                    {
                        var error = response.Content.ReadAsStringAsync().Result;
                        return (false, $"Failed to trigger pipeline. Status: {response.StatusCode}. Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error triggering Azure DevOps pipeline: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the Azure DevOps configuration (useful if you need to change settings at runtime)
        /// </summary>
        public void UpdateConfiguration(string organization, string project, int pipelineId, string pat, string branch = "refs/heads/main")
        {
            // This method can be used to update configuration if needed
            // For now, configuration is hardcoded in the private fields above
        }
    }
}
