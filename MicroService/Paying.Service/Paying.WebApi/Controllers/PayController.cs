using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Paying.WebApi.Dtos;
using Paying.WebApi.Services;
using RabbitMQ.Client;

namespace Paying.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PayController : ControllerBase
{

    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly ILogger<PayController> _logger;
    private readonly IPayingService _payingService;
    public PayController(ILogger<PayController> logger,IPayingService payingService)
    {
        _logger = logger;
        _payingService = payingService;
        RabbitMqConfig rabbitMqConfig = new();
        ConnectionFactory factory = new ConnectionFactory();
        factory.HostName = rabbitMqConfig.Host;
        factory.Port = rabbitMqConfig.Port;
        factory.UserName = rabbitMqConfig.UserName;
        factory.Password = rabbitMqConfig.Password;
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        var queueName = Const.Normal_Queue; ;
        channel.ExchangeDeclare(Const.Normal_Exchange, ExchangeType.Fanout, true);
        channel.QueueDeclare(queueName, true, false, false, new Dictionary<string, object>
                        {
                            { "x-message-ttl" ,Const.DelayTime},
                            {"x-dead-letter-exchange",Const.Delay_Exchange },
                            {"x-dead-letter-routing-key",Const.Delay_RoutingKey }
                        });

        channel.QueueBind(queueName, Const.Normal_Exchange, "");
    }


    [HttpPost]
    [Route("Paying")]
    public async Task<bool> Paying(long orderId,decimal amount)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(new CreateOrderEvent(orderId,1,1,1)));
        IBasicProperties basicProperties = channel.CreateBasicProperties();
        basicProperties.DeliveryMode = 2; //�־û�  1=�ǳ־û�
        channel.BasicPublish(Const.Normal_Exchange, Const.Normal_RoutingKey, basicProperties, buffer);
        await _payingService.Add(orderId); //�ǳ���
        _logger.LogInformation("֧���ɹ�");
        // �ڷ������ʱ�ر����Ӻ�ͨ��
        channel.Close();
        connection.Close();
        //֧������,��ʵ��ҵ��
        return true;
    }

}
