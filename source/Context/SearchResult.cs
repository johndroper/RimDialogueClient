using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Context
{
  public sealed class SearchResult<T>
  {
    public T MetaData { get; private set; }
    public float Score { get; set; }
    public SearchResult(T metaData, float score) { MetaData = metaData; Score = score; }
  }
}
