// Deterministic process-protocol fixture; this is not an antivirus engine.
var bytes = await File.ReadAllBytesAsync(args.Last());
var content = System.Text.Encoding.UTF8.GetString(bytes);
return content.Contains("SCAN_ERROR") ? 2 : content.Contains("EICAR") ? 1 : 0;
