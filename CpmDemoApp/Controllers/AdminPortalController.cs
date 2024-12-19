using CpmDemoApp.Models;
using CpmDemoApp.NugetOperations;
using CpmDemoApp.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CpmDemoApp.Controllers
{
    [Authorize]
    [Route("conversationAdmin")]
    public class AdminPortalController : Controller
    {
        private NugetRequests _nugetRequests = new NugetRequests();

        [Route("index")]
        public ActionResult display()
        {
            return View("AdminPortal");
        }

        [Route("createform")]
        public ActionResult CreateForm(string From, string MessageContent)
        {
            var conversation = new CreateConversationForm();
            conversation.CustomerPhoneNumber = From;
            conversation.InitialIncomingMessage = MessageContent; ;

            return PartialView("CreateConversation",conversation);
        }

        [Route("create")]
        [HttpPost]
        public async Task<JsonResult> Create(CreateConversationForm model)
        {
            string resultMessage;
            bool isSuccess = false;

            try
            {
                var conversation = new Conversation
                {
                    SelectedAgentIds = model.SelectedAgentIds,
                    ConversationCustomer = new Customer
                    {
                        PhoneNumber = model.CustomerPhoneNumber,
                    },
                    InitialIncomingMessage = model.InitialIncomingMessage,
                };

                conversation.SelectedAgents.AddRange(model.SelectedAgents);
                var id = await _nugetRequests.CreateConversation(conversation);
                conversation.Id = id;
                Data.ConversationDB.Add(id, conversation);
                Data.ExistingPhoneNumbers.TryAdd(model.CustomerPhoneNumber, id);

                var item = Data.NewIncomingMessagesListStatic.FirstOrDefault(x => x.CustomerPhoneNumber.Contains(model.CustomerPhoneNumber));
                if (item != null)
                {
                    Data.NewIncomingMessagesListStatic.Remove(item);
                }

                isSuccess = true;
                resultMessage = "Successfully created the conversation. Conversation ID is " + id;
            }
            catch (Exception ex) 
            {
                resultMessage = $"Conversation creation failed. Error: {ex.Message}";
            }

            return Json(new { success = isSuccess, message = resultMessage });
        }

        [Route("list")]
        [HttpGet]
        public async Task<ActionResult> List() 
        {
            //foreach (var agent in Data.AgentLists)
            //{
            //    var conversations = await _nugetRequests.GetConversationsForEmployee(agent.Id);
            //    foreach (var conversation in conversations)
            //    {

            //        if (!Data.ConversationDB.ContainsKey(conversation?.Id))
            //        {
            //            Data.ConversationDB.Add(conversation.Id, conversation);
            //        }
            //    }
            //}
            return PartialView("ListConversation");
        }

        [Route("delete")]
        [HttpPost]
        public async Task<JsonResult> Delete(string id)
        {
            string resultMessage;
            bool isSuccess = false;

            try
            {
                var response = await _nugetRequests.Terminate(id);

                if (response) 
                {
                    Data.ConversationDB.Remove(id); 
                    isSuccess = true;
                    resultMessage = $"Successfully terminated conversation {id}";
                }
                else
                {
                    resultMessage = "Termination failed";
                }
            }
            catch (Exception ex)
            {
                resultMessage = $"Termination failed. Error: {ex.Message}";
            }

            return Json(new { success = isSuccess, message = resultMessage });
        }

        [Route("addEmployeePage")]
        public async Task<ActionResult> AddEmployeePage(string id) 
        {
            var conversation = Data.ConversationDB[id];
            return PartialView("AddEmployee", id);
        }

        [Route("addEmployee")]
        [HttpPost]
        public async Task<JsonResult> AddEmployee(string id, List<string> SelectedAgentIds)
        {
            string resultMessage;
            bool isSuccess = false;

            try
            {

                var response = await _nugetRequests.AddAgent(id, SelectedAgentIds);
                if (response)
                {
                    foreach(var agentId in SelectedAgentIds)
                    {
                        Data.ConversationDB[id].SelectedAgentIds.Add(agentId);
                    }
                    isSuccess = true;
                    resultMessage = "Successfully added new agent(s)";
                }
                else
                {
                    resultMessage = $"Adding new agents failed.";
                }
            }
            catch (Exception ex)
            {
                resultMessage = $"Adding new agents failed. Error: {ex.Message}";
            }

            return Json(new { success = isSuccess, message = resultMessage });
        }

        [Route("removeEmployeePage")]
        public ActionResult RemoveEmployeePage(string id)
        {
            var conversation = Data.ConversationDB[id];
            return PartialView("RemoveEmployee", conversation);
        }

        [Route("removeEmployee")]
        [HttpPost]
        public async Task<ActionResult> RemoveEmployee(string id, List<string> SelectedAgentIds)
        {
            string resultMessage;
            bool isSuccess = false;

            try
            {
                var response = await _nugetRequests.RemoveAgent(id, SelectedAgentIds);
                if (response)
                {
                    foreach (var agentId in SelectedAgentIds)
                    {
                        Data.ConversationDB[id].SelectedAgentIds.Remove(agentId);
                    }

                    isSuccess = true;
                    resultMessage = "Successfully removed selected agent(s)";
                }
                else
                {
                    resultMessage = "Removing agents failed.";
                }
            }
            catch (Exception ex)
            {
                resultMessage = $"Removing agents failed. Error: {ex.Message}";
            }

            return Json(new { success = isSuccess, message = resultMessage });
        }
    }
}
