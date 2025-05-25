using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimDialogue.Core
{
  [Serializable]
  public class ThingData
  {

    public string Label = string.Empty;
    public bool Smeltable = false;
    public float Beauty = -1;
    public bool Destroyed = false;


  }
}
