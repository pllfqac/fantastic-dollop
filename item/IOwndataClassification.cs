using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOwndataClassification  {

	event CreateLoginSkillInfoDel LoginCreateEvent;

    void GetLoginOwndata(string encryData);
}
