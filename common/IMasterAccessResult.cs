using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public interface IMasterAccessResult 
{
   void ResultLoginAccess(int senderId,string result,byte[] sLevelArry);

}
