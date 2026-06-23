namespace My.ps2DLC.DLL
{
    public sealed class PS2DLCFileStatus
    {
        public PS2DLCFileStatus(string fileName, string resourceName, string category, bool isEmbedded, bool isApplied, string target, string message)
        {
            FileName = fileName;
            ResourceName = resourceName;
            Category = category;
            IsEmbedded = isEmbedded;
            IsApplied = isApplied;
            Target = target;
            Message = message;
        }

        public string FileName { get; private set; }

        public string ResourceName { get; private set; }

        public string Category { get; private set; }

        public bool IsEmbedded { get; private set; }

        public bool IsApplied { get; private set; }

        public bool IsEnabled
        {
            get { return IsEmbedded && IsApplied; }
        }

        public string Target { get; private set; }

        public string Message { get; private set; }
    }
}
