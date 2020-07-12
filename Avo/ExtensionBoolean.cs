using System; 

namespace Avo
{
    public static class ExtensionBoolean
    {
        public  static string ToYesOrNo(this bool value)
        {
            if (value)
            {
                return "Yes";
            }
            return "No";
        }

        public static string ToDefaultValue(this bool value, string yes, string no)
        {
            if (value)
            {
                return yes;
            }
            return no;
        }

        public static bool IsMale(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception("gender is empty");
            }
            else
            {
                value = value.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("gender is empty");
                }
            }

            if(value.ToLower() == "female") { return false;  }
            if(value.ToLower() == "f") { return false;  }
            if(value.ToLower() == "girl") { return false;  }
            if(value.ToLower() == "women") { return false;  }
            if(value.ToLower() == "woman") { return false;  }
            if(value.ToLower() == "false") { return false;  }
            if(value.ToLower() == "no") { return false;  }

            if(value.ToLower() == "man") { return true;  }
            if(value.ToLower() == "men") { return true;  }
            if(value.ToLower() == "male") { return true;  }
            if(value.ToLower() == "boy") { return true;  }
            if(value.ToLower() == "m") { return true;  }
            if(value.ToLower() == "true") { return true;  }
            if(value.ToLower() == "yes") { return true;  }


            throw new Exception("gender not found");
        }
    }
}
