using System.Threading.Tasks;
namespace PPTController.Tasks
{
    public interface ISendCommandTask
    {
        void Send(string txtCommand);
        void Connect(object obj);
        event Delagates.ResponseReceivedEventHandler ResponseReceived;
    }
}
