using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;
using System.IO;

public class DataLogger : MonoBehaviour
{
    [SerializeField]
    string number = "one";

    string directory = "C:\\Users\\Computing\\Documents\\GitHub\\studyLogs\\";
    string filename = "\\data";

    TextWriter textWriter;
    CsvWriter csvWriter;

    TransformData data;

    private void Start()
    {
        directory += number;
        Directory.CreateDirectory(directory);
        directory += filename;
        textWriter = File.CreateText(directory);
        csvWriter = new CsvWriter(textWriter);
        csvWriter.Configuration.RegisterClassMap<TransformDataMap>();
        csvWriter.WriteHeader<TransformData>();
        csvWriter.NextRecord();
    }

    public void FixedUpdate()
    {
        data = new TransformData(0f, "", false);
        csvWriter.WriteRecord(data);
        csvWriter.NextRecord();
    }

    public void Log(string value, bool continuous)
    {
        data = new TransformData(Time.time, value, continuous);
        csvWriter.WriteRecord(data);
        csvWriter.NextRecord();
    }
}

public class TransformData
{
    public float Time { get; set; }
    public string Gesture { get; set; }
    public bool Continuous { get; set; }

    public TransformData(float time, string gesture, bool continuous)
    {
        Time = time;
        Gesture = gesture;
        Continuous = continuous;
    }
}

public sealed class TransformDataMap : ClassMap<TransformData>
{
    public TransformDataMap()
    {
        Map(m => m.Time).Index(0);
        Map(m => m.Gesture).Index(1);
        Map(m => m.Continuous).Index(2);
    }
}