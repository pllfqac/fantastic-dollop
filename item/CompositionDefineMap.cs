using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Item合成の定義表.
/// Scriptable.
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create CompositionDefineMap", fileName = "CompositionDefineMap")]

public class CompositionDefineMap : ScriptableObject
{
	[SerializeField]
	private List<CompositionDefine> recipes = new List<CompositionDefine>();



	//現状recipeはひとつに絞られるようにしなければならない仕様
	//合成の原料の組み合わせから対応するCompositionDefineを取得する.存在しなければnull.
	public CompositionDefine GetRecipe(Dictionary<int, OneAbsItemClass> selectedItem)
	{
		List<CompositionDefine> result = null;

		int i = 0;
		foreach (int abs in selectedItem.Keys)
		{
			//最初の検索はrecipesの中から,2回目以降のAbs検索は以前の検索で選ばれたListの中から取得.
			if (i == 0)
			{
				result = recipes.Where(x => x.materials.Any(a => a.d == abs)).ToList();
			}
			else
			{
				result = result.Where(x => x.materials.Any(a => a.d == abs)).ToList();
			}
			Debug.Log("roop Count:" + i + "  検索Abs:" + abs + "  recipe Count:" + result.Count);
			i++;
		}
        //recipeが一つに絞れないときは原料を満たすもののみを選択.
        if (result.Count > 1)
        {
			//return SelectOneRecipe(result);
			result = SelectOneRecipe(result);
		}

		if (result.Count == 1 && CheckLack(result.First(), selectedItem)) return result.First();
		else return null;
	}

	/// <summary>
	/// recipeの絞り込み.原料がすべて存在するもの(=必要な原料の種類がいちばん小さいもの)のみを選択する.
	/// </summary>
	/// <param name="recipes"></param>
	/// <returns></returns>
	private List< CompositionDefine> SelectOneRecipe(List<CompositionDefine> recipes)
    {
		CompositionDefine result=null;
		int tempMaterialCount = int.MaxValue;
		foreach(CompositionDefine cDef in recipes)
        {
			int count = cDef.materials.Count();
			Debug.Log("Material count:" + count);
			if (tempMaterialCount > count)
			{
				tempMaterialCount = count;
				result = cDef;
				Debug.Log("tempCount:" + tempMaterialCount);
			}
		}
		return new List<CompositionDefine>() { result};
    }


	/// <summary>
	/// selectされたItemの不足確認.
	/// Recipeに記入された全AbsがSelectされたitemに存在するか
	/// </summary>
	/// <returns>存在ならtrue.1つでも不足ならfalse.</returns>
	private bool CheckLack(CompositionDefine recipe, Dictionary<int, OneAbsItemClass> selectedItem)
    {
		foreach(var sc in recipe.materials)
        {
			if (!selectedItem.Keys.Any(x => x == sc.d)) return false;
        }
		return true;
    }
}
