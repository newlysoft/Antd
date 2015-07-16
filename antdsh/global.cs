﻿namespace antdsh {
    public class global {
        public static string versionsDir { get { return "/mnt/cdrom/DIRS/DIR_framework_antd_versions"; } }
        public static string tmpDir { get { return "/mnt/cdrom/DIRS/DIR_framework_antd_tmp"; } }
        public static string appsDir { get { return "/mnt/cdrom/Apps"; } }
        public const string configFile = "antdsh.config";
        public const string antdRunning = "running";
        public const string downloadName = "antdDownload.zip";
        public const string downloadFirstDir = "antdDownloadFirst";

        public const string zipStartsWith = "antd";
        public const string zipEndsWith = ".7z";
        public const string squashStartsWith = "DIR_framework_antd";
        public const string squashEndsWith = ".squashfs.xz";

        public const string dateFormat = "yyyyMMdd";
    }
}
