#include <stdio.h>
#include <core_expt.h>
#include <Windows.h>
#using <System.dll>

using namespace System;
using namespace System::Text;
using namespace System::IO;
using namespace System::Net;
using namespace System::Net::Sockets;

#define DURATION 20000 //20 seconds

String ^ generateString(float hx1, float hx2, float hy1, float hy2)
{
	String ^ message = "";
	message += hx1.ToString() + "," + hx2.ToString() + "," + hy1.ToString() + "," + hy2.ToString() + "\n";
	return message;
}

int main(int argc, char ** argv)
{
	String ^ iPAddress = "128.16.6.112"; //"192.168.2.67"
	int port = 13000;

	TcpClient ^ client;
	NetworkStream ^ stream;

	try
	{
		// Create a TcpClient.
		// Note, for this client to work you need to have a TcpServer 
		// connected to the same address as specified by the server, port
		// combination.

		Console::WriteLine("Attempting to connect to EyeLink...");
		client = gcnew TcpClient(iPAddress, port);
		stream = client->GetStream();

		// Get a client stream for reading and writing.
		//Stream stream = client->GetStream();

	}
	catch (ArgumentNullException^ e)
	{
		Console::WriteLine("ArgumentNullException: {0}", e);

		return -1;
	}
	catch (SocketException^ e)
	{
		Console::WriteLine("SocketException: {0}", e);

		return -1;
	}

	if (open_eyelink_connection(0) != 0) // connect to the tracker
	{
		printf("Failed to connect to tracker \n");
		return 0;
	}

	eyecmd_printf("link_sample_data  = LEFT,RIGHT,HREF"); // tell tracker to send data over the link
	eyecmd_printf("binocular_enabled =YES"); // enable binocular
	if (start_recording(1, 1, 1, 1) != 0)
	{
		printf("failed to start recording \n");
		return -1;
	}

	String ^ message;
	FSAMPLE sample;
	while (!(GetAsyncKeyState(VK_SPACE) & 0x8000))
	{
		if (eyelink_newest_float_sample(&sample) > 0) // get the newest sample
		{
			message = generateString(sample.hx[0], sample.hx[1], sample.hy[0], sample.hy[1]); // form the gaze data

			// Translate the passed message into ASCII and store it as a Byte array.
			array<Byte>^data = Text::Encoding::ASCII->GetBytes(message);
			// Send the message to the connected TcpServer. 
			stream->Write(data, 0, data->Length);

			//printf("%s", message);
		}
	}

	// Close the connection
	message = "fin\n";

	// Translate the passed message into ASCII and store it as a Byte array.
	array<Byte>^data = Text::Encoding::ASCII->GetBytes(message);
	// Send the message to the connected TcpServer. 
	stream->Write(data, 0, data->Length);

	client->Close();

	stop_recording(); // stop recording
	close_eyelink_connection(); // disconnect from tracker
	return 1;
}