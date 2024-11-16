using UnityEngine.UI;
using UnityEngine;

public interface INodeData
{
    void TapNode();
    /// <summary>
    /// Skill or Item のAbsoluteNumを返す.
    /// </summary>
    /// <returns>AbsoluteNum</returns>
    int GetAboluteNum();

    /// <summary>
    /// Skill用.
    /// SkillInfoClassの参照を持っていればそれを返す.
    /// </summary>
    SkillInfoClass GetSkillInfo();

    /// <summary>
    /// Item用.
    /// </summary>
    /// <returns></returns>
    OneAbsItemClass GetOneAbsItemClass();

    /// <summary>
    /// Item用.
    /// </summary>
    /// <returns></returns>
    OneItemClass GetOneItemClass();

    /// <summary>
    /// PrefsSaveDataClassで定義されたNodeのTypeを返す.
    /// </summary>
    /// <returns>このNodeのType.</returns>
    PrefsSaveDataClass.NodeType GetNodeType();

    /// <summary>
    /// Hash付Itemの場合のみGUIDを返す.無ければnull.
    /// </summary>
    /// <returns>ItemのGUID</returns>
    string GetGUID();

    Image GetNodeImage();                       //Node,ItemNodeプレファブの子のNodeImageについているImageコンポーネントを取得.
    void SetActive(bool b);                     //Nodeの表示非表示.
    void ReturnPool();                          //Poolへモドス.
    Text countText { get; set; }                        //Item用.個数用Textの参照を取得.

	//  void UseItem();                     //Itemの使用.

	/// <summary>
	/// Skill用.
	/// NodeのSkillInfoClassを変更する.
	/// </summary>
	/// <param name="newSkillInfo">変更したいSkillInfoClass.</param>
	void SetSkillInfo(SkillInfoClass newSkillInfo);

	/// <summary>
	/// 使用できないSkillのNode,Iconの色を変えてUserに伝える.
	/// </summary>
	void HideDisabledSkillNode();

	/// <summary>
	/// HideDisabledSkillNode()で変更した色をもとに戻す.
	/// </summary>
	void ShowEnableSkillNode();

	//SKill用.このNodeのSkillが現在装備中のWeapon用かどうか
	Color GetNodeColor();

    bool IsDragging();
}
