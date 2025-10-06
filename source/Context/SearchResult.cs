namespace RimDialogue.Context
{
  public sealed class SearchResult<T>
  {
    public T MetaData { get; private set; }
    public float Score { get; set; }
    public SearchResult(T metaData, float score) { MetaData = metaData; Score = score; }
  }
}
