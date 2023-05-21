using System;

namespace WpfClient
{
    public class Constants
    {
        public const String SERVER_PORT = "2000"; // Socket port to connect
        public const String MJPEG_PORT = "5001"; // USB port to connect
        // String to recognize command
        public const String COMMAND_IDENTIFIER = "command/";
        // String to recognize Image Update request
        public const String IMAGE_UPDATE_REQUEST = "start_update";
        // String to recognize Image Update request
        public const String IMAGE_UPDATE_STOP = "stop_update";
        // Server ping interval
        public const int SERVER_PING_INETRVAL = 2000;
        // Buffer size
        public const int MAX_BUFFER_SIZE = 1024 * 64;

        //MJPEG  Sizes
        public const int CHUNK_SIZE = 1024;
        public const int MAX_IMAGE_BUFFER_SIZE = 1024 * 1024;

        public const string APP_NAME = "VRConsole";
        public const int LOG_BACKUP_DAYS = 30;

        // GUI update status
        public enum GUI_STATUS_e
        {
            GUI_STATUS_CONNECTION_LOST = 0,
            GUI_STATUS_CONNECTED,
            GUI_STATUS_LOCK_ON,
            GUI_STATUS_LOCK_OFF
        };
    }
}