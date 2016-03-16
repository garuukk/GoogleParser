using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Svyaznoy.Core.Web.Soap
{
    public class SoapClient<TClient, TChannel>
        where TChannel: class
        where TClient: ClientBase<TChannel>
    {
        public TClient Client { get; private set; }

        public string LastRequest
        {
            get { return _logEndpointBehavior.LogMessageInspector.LastRequest; }
        }

        public string LastResponse
        {
            get { return _logEndpointBehavior.LogMessageInspector.LastResponse; }
        }

        private readonly LogEndpointBehavior _logEndpointBehavior = new LogEndpointBehavior();

        public SoapClient(TClient client, bool openClient = true)
        {
            if (client == null) throw new ArgumentNullException("client");

            Client = client;

            client.Endpoint.Behaviors.Add(_logEndpointBehavior);

            if (openClient)
            {
                client.Open();
            }
        }



        public class LogMessageInspector : IClientMessageInspector
        {
            public string LastRequest { get; private set; }

            public string LastResponse { get; private set; }

            public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
            {
                LastResponse = reply.ToString();
            }

            public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
            {
                LastRequest = request.ToString();
                LastResponse = null;
                return null;
            }
        }

        // Endpoint behavior
        public class LogEndpointBehavior : IEndpointBehavior
        {
            public readonly LogMessageInspector LogMessageInspector = new LogMessageInspector();
            public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
            {
                // No implementation necessary
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(LogMessageInspector);
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                // No implementation necessary
            }

            public void Validate(ServiceEndpoint endpoint)
            {
                // No implementation necessary
            }
        }
    }
}
