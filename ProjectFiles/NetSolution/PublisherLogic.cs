#region StandardUsing
using System;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.OPCUAServer;
#endregion
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class PublisherLogic : BaseNetLogic
{
    public override void Start()
    {
        
        var brokerIpAddressVariable = Project.Current.GetVariable("model/BrokerIp");

        // Create a client connecting to the broker (default port is 1883)
        publishClient = new MqttClient(brokerIpAddressVariable.Value);
        // Connect to the broker
        publishClient.Connect("FTOptixPublishClient");
        // Assign a callback to be executed when a message is published to the broker
        publishClient.MqttMsgPublished += PublishClientMqttMsgPublished;
    }

    public override void Stop()
    {
        publishClient.Disconnect();
        publishClient.MqttMsgPublished -= PublishClientMqttMsgPublished;
    }

    private void PublishClientMqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
    {
        Log.Info("Message " + e.MessageId + " - published = " + e.IsPublished);
    }

    [ExportMethod]
    public void PublishMessage()
    {
        var topic = Project.Current.GetVariable("Model/Topic");
        var data = Project.Current.GetVariable("Model/Topic");
        

        // Publish a message
        ushort msgId = publishClient.Publish(topic.ToString(), // topic
            System.Text.Encoding.UTF8.GetBytes(((int)data.Value).ToString()), // message body
            MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
            false); // retained
    }

    private MqttClient publishClient;
}
