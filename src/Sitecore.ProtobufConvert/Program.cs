namespace ProtobufConvert
{
  public static class Program
  {                    
    public static void Main(string[] args)
    {
      var parser = new Fclp.FluentCommandLineParser<ConvertCommand>();

      parser.Setup(x => x.ConnectionString)
        .As('c', "connectionString")
        .Required();

      parser.Setup(x => x.DatabaseName)
        .As('n', "databaseName")
        .Required();

      parser.Setup(x => x.OutputDirectory)
        .As('o', "outputDirectory")        
        .Required();                     

      var result = parser.Parse(args);

      if (!result.HasErrors)
      {
        var command = parser.Object;   
                                  
        command.Execute();
      }
    }
  }      
}
