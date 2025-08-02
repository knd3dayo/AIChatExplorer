using System.Net.Http;
using System.Text;
using System.Text.Json;
using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.File;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.Statistics;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.PythonIF {

    public class PythonAPIFunctions : IPythonAIFunctions {

        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        private static HttpClient CreateHttpClient() {
            HttpClient client = new(new HttpClientHandler() {
                UseProxy = false,
                // 最大接続数を設定
                MaxConnectionsPerServer = 2,
            });
            // タイムアウトを設定
            client.Timeout = TimeSpan.FromMinutes(5);

            return client;
        }

        private string base_url;
        public PythonAPIFunctions(string base_url) {
            this.base_url = base_url;
        }

        // HttpClient
        public static async Task<string> PostAsync(string endpoint, string requestJson) {
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrEmpty(requestJson))
                throw new ArgumentNullException(nameof(requestJson));
            var data = new StringContent(requestJson, Encoding.UTF8, mediaType: "application/json");
            using HttpClient client = CreateHttpClient();
            var response = await client.PostAsync(endpoint, data).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }



        // テスト用
        public async Task<string> HelloWorldAsync() {
            string url = $"{this.base_url}/hello_world";
            return await PostAsync(url, "{}");
        }

        // テスト用
        public static void ShutdownServer(string url) {
            // ClientでPOSTを実行
            using HttpClient client = CreateHttpClient();
            client.PostAsync(url, new StringContent("{}", Encoding.UTF8, mediaType: "application/json"));

        }

        // ContentItem
        public async Task<ContentItemEntity> GetContentItemByIdAsync(string id) {
            // RequestContainerを作成
            ContentItemEntity contentItemEntity = new() { Id = id };
            RequestContainer requestContainer = new() {
                ContentItemRequestsInstance = [new ContentItemRequest(contentItemEntity)]
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetContentItemExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{base_url}/get_content_item";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            // content_itemを取得
            dynamic? dict = resultDict.GetValueOrDefault("content_item", null);
            if (dict == null)
                return new ContentItemEntity();
            // ContentItemWrapperを取得
            ContentItemResponse contentItemResponse = ContentItemResponse.FromDict(dict);
            return contentItemResponse.Entity;
        }

        public async Task<List<ContentItemEntity>> GetContentItemsAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetContentItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_content_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic? dictList = resultDict.GetValueOrDefault("content_items", "[]");
            List<ContentItemEntity> contentItems = [];
            if (dictList == null)
                return contentItems;
            foreach (var item in dictList) {
                ContentItemResponse contentItemResponse = ContentItemResponse.FromDict(item);
                if (contentItemResponse.Entity != null)
                    contentItems.Add(contentItemResponse.Entity);
            }
            return contentItems;
        }
        public async Task<List<ContentItemEntity>> GetContentItemsByFolderAsync(string folderId) {
            ContentItemWrapper contentItemWrapper = new() { FolderId = folderId };
            ContentItemRequest contentItemRequest = new(contentItemWrapper.Entity);
            RequestContainer requestContainer = new() {
                ContentItemRequestsInstance = [contentItemRequest]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetContentItemsByFolderExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_content_items_by_folder_id";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["content_items"] ?? "[]";
            List<ContentItemEntity> contentItems = [];
            foreach (var item in dictList) {
                ContentItemResponse contentItemResponse = ContentItemResponse.FromDict(item);
                if (contentItemResponse.ContentItem != null)
                    contentItems.Add(contentItemResponse.Entity);
            }
            return contentItems;
        }

        public async Task UpdateContentItemAsync(List<ContentItemRequest> requests) {
            RequestContainer requestContainer = new() {
                ContentItemRequestsInstance = requests
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateContentItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_content_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task DeleteContentItemsAsync(List<ContentItemRequest> requests) {
            RequestContainer requestContainer = new() {
                ContentItemRequestsInstance = requests
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteContentItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_content_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }
        // SearchContentItems
        public async Task<List<ContentItemEntity>> SearchContentItems(SearchCondition searchCondition) {
            RequestContainer requestContainer = new() {
                SearchRequestInstance = searchCondition
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SearchContentItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/search_content_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["content_items"] ?? "[]";
            List<ContentItemEntity> contentItems = [];
            foreach (var item in dictList) {
                ContentItemResponse contentItemResponse = ContentItemResponse.FromDict(item);
                if (contentItemResponse.Entity != null)
                    contentItems.Add(contentItemResponse.Entity);
            }
            return contentItems;
        }

        // ContentFolder
        public async Task<List<ContentFolderEntity>> GetRootContentFoldersAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetRootContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_root_content_folders";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["content_folders"] ?? "[]";
            List<ContentFolderEntity> contentFolders = [];
            foreach (var item in dictList) {
                ContentFolderEntity? contentFolderEntity = ContentFolderEntity.FromDict(item);
                if (contentFolderEntity != null)
                    contentFolders.Add(contentFolderEntity);
            }
            return contentFolders;
        }
        public async Task<List<ContentFolderEntity>> GetContentFoldersAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_content_folders";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["content_folders"] ?? "[]";
            List<ContentFolderEntity> contentFolders = [];
            foreach (var item in dictList) {
                ContentFolderEntity? contentFolderEntity = ContentFolderEntity.FromDict(item);
                if (contentFolderEntity != null)
                    contentFolders.Add(contentFolderEntity);
            }
            return contentFolders;
        }
        public async Task<ContentFolderEntity> GetContentFolderByIdAsync(string id) {
            ContentFolderEntity contentFolderEntity = new() { Id = id };
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = [new ContentFolderRequest(contentFolderEntity)]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetContentFolderByIdExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_content_folder_by_id";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic? dict = resultDict.GetValueOrDefault("content_folder", null);
            if (dict == null)
                return new ContentFolderEntity();
            ContentFolderResponse contentFolderResponse = ContentFolderResponse.FromDict(dict);
            return contentFolderResponse.Entity;
        }
        public async Task<ContentFolderEntity> GetContentFolderByPathAsync(string name) {
            string chatRequestContextJson = "{\"content_folder_path\":\"" + name + "\"}";
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetContentFolderByPathExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_content_folder_by_path";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic? dict = resultDict.GetValueOrDefault("content_folder", null);
            if (dict == null)
                return new ContentFolderEntity();
            ContentFolderResponse contentFolderResponse = ContentFolderResponse.FromDict(dict);
            return contentFolderResponse.Entity;
        }
        public async Task<ContentFolderEntity?> GetParentFolderByIdAsync(string id) {
            ContentFolderEntity contentFolderEntity = new() { Id = id };
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = [new ContentFolderRequest(contentFolderEntity)]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetParentContentFolderExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_parent_content_folder_by_id";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic? dict = resultDict.GetValueOrDefault("content_folder", null);
            if (dict == null)
                return null;
            ContentFolderResponse contentFolderResponse = ContentFolderResponse.FromDict(dict);
            return contentFolderResponse.Entity;
        }

        public async Task<List<ContentFolderEntity>> GetChildFoldersByIdAsync(string id) {
            ContentFolderEntity contentFolderEntity = new() { Id = id };
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = [new ContentFolderRequest(contentFolderEntity)]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetChildContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_child_content_folders_by_id";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["content_folders"] ?? "[]";
            List<ContentFolderEntity> contentFolders = [];
            foreach (var item in dictList) {
                ContentFolderResponse contentFolderResponse = ContentFolderResponse.FromDict(item);
                if (contentFolderResponse.Entity != null)
                    contentFolders.Add(contentFolderResponse.Entity);
            }
            return contentFolders;
        }


        public async Task UpdateContentFoldersAsync(List<ContentFolderRequest> folders) {
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = folders
            };
            string chatRequestContextJson = requestContainer.ToJson();
            // LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_content_folders";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task DeleteContentFoldersAsync(List<ContentFolderRequest> folders) {
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = folders
            };
            string chatRequestContextJson = requestContainer.ToJson();
            // LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_content_folders";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // SearchRule
        public async Task<List<SearchRule>> GetSearchRulesAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetSearchRulesExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_search_rules";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["search_rules"] ?? "[]";
            List<SearchRule> searchRules = [];
            foreach (var item in dictList) {
                SearchRule? searchRule = SearchRule.FromDict(item);
                if (searchRule != null)
                    searchRules.Add(searchRule);
            }
            return searchRules;
        }

        public async Task UpdateSearchRuleAsync(SearchRuleRequest request) {
            RequestContainer requestContainer = new() {
                SearchRuleRequestsInstance = [request]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateSearchRulesExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_search_rules";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task DeleteSearchRuleAsync(SearchRuleRequest request) {
            RequestContainer requestContainer = new() {
                SearchRuleRequestsInstance = [request]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteSearchRulesExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_search_rules";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }


        // AutoProcessItem
        public async Task<List<AutoProcessItem>> GetAutoProcessItemsAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetAutoProcessItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_auto_process_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["auto_process_items"] ?? "[]";
            List<AutoProcessItem> autoProcessItems = [];
            foreach (var item in dictList) {
                AutoProcessItem? autoProcessItem = AutoProcessItem.FromDict(item);
                if (autoProcessItem != null)
                    autoProcessItems.Add(autoProcessItem);
            }
            return autoProcessItems;
        }

        public async Task UpdateAutoProcessItemAsync(AutoProcessItemRequest request) {
            RequestContainer requestContainer = new() {
                AutoProcessItemsInstance = [request]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateAutoProcessItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_auto_process_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task DeleteAutoProcessItemAsync(AutoProcessItemRequest request) {
            RequestContainer requestContainer = new() {
                AutoProcessItemsInstance = [request]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteAutoProcessItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_auto_process_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // AutoProcessRule
        public async Task<List<AutoProcessRule>> GetAutoProcessRulesAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetAutoProcessRulesExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_auto_process_rules";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["auto_process_rules"] ?? "[]";
            List<AutoProcessRule> autoProcessRules = [];
            foreach (var item in dictList) {
                AutoProcessRule? autoProcessRule = AutoProcessRule.FromDict(item);
                if (autoProcessRule != null)
                    autoProcessRules.Add(autoProcessRule);
            }
            return autoProcessRules;
        }

        public async Task UpdateAutoProcessRuleAsync(AutoProcessRuleRequest rule) {
            RequestContainer requestContainer = new() {
                AutoProcessRulesInstance = [rule]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateAutoProcessRulesExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_auto_process_rules";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task DeleteAutoProcessRuleAsync(AutoProcessRuleRequest rule) {
            RequestContainer requestContainer = new() {
                AutoProcessRulesInstance = [rule]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteAutoProcessRulesExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_auto_process_rules";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }


        public async Task<List<PromptItem>> GetPromptItemsAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetPromptItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_prompt_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["prompt_items"] ?? "[]";
            List<PromptItem> promptItems = [];
            foreach (var item in dictList) {
                PromptItem? promptItem = PromptItem.FromDict(item);
                if (promptItem != null)
                    promptItems.Add(promptItem);
            }
            return promptItems;
        }

        public async Task UpdatePromptItemAsync(PromptItemRequest request) {
            RequestContainer requestContainer = new() {
                PromptItemsInstance = [request]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdatePromptItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_prompt_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task DeletePromptItemAsync(PromptItemRequest request) {
            RequestContainer requestContainer = new() {
                PromptItemsInstance = [request]
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeletePromptItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_prompt_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // GetTagItemsAsync
        public async Task<List<TagItem>> GetTagItemsAsync() {
            RequestContainer requestContainer = new();
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/get_tag_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
            dynamic dictList = resultDict["tag_items"] ?? "[]";
            List<TagItem> tagItems = [];
            foreach (var item in dictList) {
                TagItem? tagItem = TagItem.FromDict(item);
                if (tagItem != null)
                    tagItems.Add(tagItem);
            }
            return tagItems;
        }
        // UpdateTagItemsAsync
        public async Task UpdateTagItemsAsync(List<TagItem> tagItems) {
            RequestContainer requestContainer = new() {
                TagItemsInstance = tagItems
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_tag_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // DeleteTagItemsAsync
        public async Task DeleteTagItemsAsync(List<TagItem> tagItems) {
            RequestContainer requestContainer = new() {
                TagItemsInstance = tagItems
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_tag_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // GetTokenCount
        public async Task<long> GetTokenCount(ChatRequestContext chatRequestContext, TokenCountRequest tokenCountRequest) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                TokenCountRequestInstance = tokenCountRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetTokenCountExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            long totalTokens = 0;
            // PostAsyncを実行する
            string endpoint = $"{base_url}/get_token_count";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // total_tokensを取得
            if (resultDict.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                if (totalTokensValue is decimal totalTokensDecimal) {
                    totalTokens = decimal.ToInt64(totalTokensDecimal);
                }
            }
            return totalTokens;
        }

        // 通常のOpenAIChatを実行する
        public async Task<ChatResponse> OpenAIChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ChatRequestInstance = chatRequest,
                RequestContextInstance = chatRequestContext,
            };
            // RequestContainerをJSON文字列に変換
            string requestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.OpenAIExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {requestContextJson}");

            // ChatResultを作成
            ChatResponse chatResult = new();
            // PostAsyncを実行する
            string endpoint = $"{base_url}/openai_chat";
            string resultString = await PostAsync(endpoint, requestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

            chatResult.LoadFromJson(resultString);

            // Errorがある場合は例外をスローする
            if (!string.IsNullOrEmpty(chatResult.Error)) {
                throw new Exception(chatResult.Error);
            }
            // StatisticManagerにトークン数を追加
            MainStatistics.GetMainStatistics().AddTodayTokens(chatResult.TotalTokens, chatRequest.Model);
            return chatResult;
        }
        public async Task<string> ExtractFileToTextAsync(string path) {
            // FileRequestを作成
            FileRequest fileRequest = new() {
                FilePath = path
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                FileRequestInstance = fileRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            PythonScriptResult result = new();
            // PostAsyncを実行する
            string endpoint = $"{base_url}/extract_text_from_file";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        public async Task<string> ExtractBase64ToText(string base64, string extension) {
            // FileRequestを作成
            FileRequest fileRequest = new() {
                Base64Data = base64,
                Extension = extension
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                FileRequestInstance = fileRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");

            // ResultContainerを作成
            PythonScriptResult result = new();
            // PostAsyncを実行する
            string endpoint = $"{base_url}/extract_base64_to_text";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        public async Task UpdateVectorDBItemAsync(VectorDBItem item) {
            RequestContainer requestContainer = new() {
                VectorDBItemRequestInstance = new VectorDBItemRequest(item)
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateVectorDBItemExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_vector_db_item";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // ベクトルDBを削除する
        public async Task DeleteVectorDBItemAsync(VectorDBItem item) {
            RequestContainer requestContainer = new() {
                VectorDBItemRequestInstance = new VectorDBItemRequest(item)
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteVectorDBItemExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_vector_db_item";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // public List<VectorDBItem> GetVectorDBItemsAsync();
        public async Task<List<VectorDBItem>> GetVectorDBItemsAsync() {
            // RequestContainerを作成
            RequestContainer requestContainer = new();
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetVectorDBItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{base_url}/get_vector_db_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

            // VectorDBItemのリストを取得
            dynamic dictList = resultDict["vector_db_items"] ?? "[]";
            List<VectorDBItem> vectorDBItems = [];
            foreach (var item in dictList) {
                // VectorDBItemを取得
                VectorDBItem? vectorDBItem = VectorDBItemResponse.FromDict(item).CreateVectorDBItem();
                if (vectorDBItem != null) {
                    vectorDBItems.Add(vectorDBItem);
                }
            }
            return vectorDBItems;
        }
        // public VectorDBItem? GetVectorDBItemById(string id);
        public async Task<VectorDBItem?> GetVectorDBItemById(string id) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                VectorDBItemRequestInstance = new VectorDBItemRequest(new VectorDBItem() { Id = id })
            };

            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetVectorDBItemByIdExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{base_url}/get_vector_db_item_by_id";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

            // VectorDBItemを取得
            VectorDBItem? vectorDBItem = VectorDBItemResponse.FromDict(resultDict["vector_db_item"] ?? "[]").CreateVectorDBItem();
            return vectorDBItem;
        }
        // public VectorDBItem? GetVectorDBItemByName(string name);
        public async Task<VectorDBItem?> GetVectorDBItemByName(string name) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                VectorDBItemRequestInstance = new VectorDBItemRequest(new VectorDBItem() { Name = name })
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetVectorDBItemByNameExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{base_url}/get_vector_db_item_by_name";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

            // VectorDBItemを取得
            VectorDBItem? vectorDBItem = VectorDBItemResponse.FromDict(resultDict["vector_db_item"] ?? "[]").CreateVectorDBItem();
            return vectorDBItem;
        }

        public async Task<List<VectorEmbeddingItem>> VectorSearchAsync(ChatRequestContext chatRequestContext, string query) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.VectorSearchExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.VectorSearchRequest}:{query}");

            // vector_search

            // PostAsyncを実行する
            string endpoint = $"{base_url}/vector_search";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            if (resultDict == null) {
                throw new Exception(StringResources.OpenAIResponseEmpty);
            }

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

            List<VectorEmbeddingItem> vectorSearchResults = new();
            List<string> textResults = new();
            // documentsがある場合は取得
            var documents = resultDict.GetValueOrDefault("documents") as List<dynamic> ?? throw new Exception(StringResources.OpenAIResponseEmpty);
            if (documents == null) {
                throw new Exception(StringResources.OpenAIResponseEmpty);
            }
            foreach (var item in documents) {
                VectorSearchResponse? VectorEmbeddingItem = VectorSearchResponse.FromDict(item);
                if (VectorEmbeddingItem != null) {
                    vectorSearchResults.Add(VectorEmbeddingItem.CreateVectorEmbeddingItem());
                }
            }
            return vectorSearchResults;
        }


        public async Task DeleteEmbeddingsByFolderAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                EmbeddingRequestInstance = embeddingRequest
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteEmbeddingsByFolderExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_embeddings_by_folder";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task DeleteEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                EmbeddingRequestInstance = embeddingRequest
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateEmbeddingExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/delete_embeddings";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        public async Task UpdateEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                EmbeddingRequestInstance = embeddingRequest
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateEmbeddingExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
            string endpoint = $"{base_url}/update_embeddings";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // ExportToExcelを実行する
        public async Task ExportToExcelAsync(string filePath, CommonDataTable data) {
            ExcelRequest excelRequest = new(filePath, data);
            RequestContainer requestContainer = new() {
                ExcelRequestInstance = excelRequest
            };
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.ExportToExcelExecute);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo}:{chatRequestContextJson}");
            string endpoint = $"{base_url}/export_to_excel";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            if (resultDict.TryGetValue("error", out dynamic? errorValue))
                throw new Exception(errorValue);
        }

        // ImportFromExcelを実行する
        public async Task<CommonDataTable> ImportFromExcel(string filePath) {
            // FileRequestを作成
            FileRequest fileRequest = new() {
                FilePath = filePath
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                FileRequestInstance = fileRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            CommonDataTable result = new([]);
            // import_from_excel

            // PostAsyncを実行する
            string endpoint = $"{base_url}/import_from_excel";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, JsonUtil.JsonSerializerOptions);
            if (resultDict == null) {
                throw new Exception(StringResources.OpenAIResponseEmpty);
            }
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

            // documents を取得
            JsonElement? documentsObject = (JsonElement)resultDict["rows"];
            if (documentsObject == null) {
                throw new Exception(StringResources.OpenAIResponseEmpty);
            }

            // JSON文字列からList<List<string>>に変換する。
            if (string.IsNullOrEmpty(resultString) == false) {
                result = CommonDataTable.FromJson(documentsObject.ToString() ?? "[]");
            }
            return result;
        }

        // GetMimeType
        public async Task<string> GetMimeType(string filePath) {
            FileRequest fileRequest = new() {
                FilePath = filePath
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                FileRequestInstance = fileRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            // ResultContainerを作成
            PythonScriptResult result = new();
            // get_mime_type
            // PostAsyncを実行する
            string endpoint = $"{base_url}/get_mime_type";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        // public string ExtractWebPage(string url);
        public async Task<string> ExtractWebPage(string url) {
            // FileRequestを作成
            WebRequest webRequest = new(url);
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                WebRequestInstance = webRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");

            // ResultContainerを作成
            PythonScriptResult result = new();
            // extract_webpage
            // PostAsyncを実行する
            string endpoint = $"{base_url}/extract_webpage";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);

            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }
    }
}
