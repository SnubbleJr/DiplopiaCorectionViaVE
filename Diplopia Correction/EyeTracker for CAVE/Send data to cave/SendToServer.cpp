#include <stdio.h>
#include "SendToServer.h"
#include "gdi_expt.h"
#using <System.dll>

using namespace System;
using namespace System::Text;
using namespace System::IO;
using namespace System::Net;
using namespace System::Net::Sockets;

void Connect(String ^ message)
{
	String ^ iPAddress = "192.168.2.67"; //"128.16.6.112"
	int port = 13000;

	try
	{
		// Create a TcpClient.
		// Note, for this client to work you need to have a TcpServer 
		// connected to the same address as specified by the server, port
		// combination.
		TcpClient^ client = gcnew TcpClient(iPAddress, port);
		// Translate the passed message into ASCII and store it as a Byte array.
		array<Byte>^data = Text::Encoding::ASCII->GetBytes(message);

		// Get a client stream for reading and writing.
		//  Stream stream = client->GetStream();

		NetworkStream^ stream = client->GetStream();

		// Send the message to the connected TcpServer. 
		stream->Write(data, 0, data->Length);

		printf("Sent: %s\n", message);

		// Receive the TcpServer::response.
		//do't care about a responce

		// Close everything.
		client->Close();
	}
	catch (ArgumentNullException^ e)
	{
		printf("ArgumentNullException: {0}", e);
	}
	catch (SocketException^ e)
	{
		printf("SocketException: {0}", e);
	}
}

//Generates the string that is used to send to the server
void sendSample(float hx1, float hx2, float hy1, float hy2)
{
	String ^ message = "";
	message += hx1.ToString() + "," + hx2.ToString() + "," + hy1.ToString() + "," + hy2.ToString();
	Connect(message);
}