using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTable {

    /// <summary>
    /// Test表示用.
    /// </summary>
    public System.Action<Dictionary<string, int>> DebugAction { get; set; }

    /// <summary>
    /// TableへUserを追加する.
    /// </summary>
    /// <param name="userId">User固有の一意の文字列.FirebaseUser.UserId(==PhotonNetwork.AuthValues.UserId)</param>
    /// <param name="obj"></param>
    void AddAllUserTable(string userId, GameObject obj);
    /// <summary>
    /// Tableから(存在すれば)取り除く.
    /// </summary>
    /// <param name="targetUserId">取り除く対象のUserId.User固有の一意の文字列.</param>
    void RemoveAllUserTable(string targetUserId);

    /// <summary>
    /// AllUserTableのSceneIndexを更新.
    /// </summary>
    /// <param name="targetUserId">指定するUserId.一意のId.存在しなければ何もしない.</param>
    /// <param name="newSceneIndex">新しいSceneIndex.</param>
    void SetSceneIndex(string targetUserId, int newSceneIndex);
    /// <summary>
    /// 指定したUserIdのSceneIndexを取得します.存在しなければ-1.
    /// </summary>
    /// <param name="targetUserId">指定するUserId.一意のId</param>
    int GetSceneIndex(string targetUserId);

    /// <summary>
    /// PhotonView.Owner.ActorNumber=>PhotonNetwork.AuthValues.UserIdへの変換.
    /// </summary>
    /// <param name="viewOwnerActorNum">PhotonView.Owner.ActorNumber</param>
    /// <returns>見つからない場合はnull返す.</returns>
    public string ConvertViewOwnerActorNumberToUserId(int viewOwnerActorNum);

    /// <summary>
    /// PhotonNetwork.AuthValues.UserId => PhotonView.Owner.ActorNumberへの変換.
    /// </summary>
    /// <param name="userId">PhotonNetwork.AuthValues.UserId</param>
    /// <returns>見つからなければ-1.</returns>
    public int ConvertUserIdToViewOwnerActorNumber(string userId);

    /// <summary>
    /// PlayerTableから指定したUserを検索して返す.
    /// 一意のID,UserIdで検索Ver.
    /// </summary>
    /// <param name="userId">一意のId.</param>
    /// <returns>なければnullと-1のタプル.</returns>
    (GameObject obj, int sceneIndex, PhotonView view) FindUserObject(string userId);

    /// <summary>
    /// PlayerTableから指定したUserを検索して返す.
    /// PhotonPlayer.Find()の代替.
    /// </summary>
    /// <param name="ownerActorNnum">抽出対象のPhotonView.Owner.ActorNumber</param>
    /// <returns>なければnullと-1のタプル.</returns>
    (GameObject obj, int sceneIndex, PhotonView view) FindUserObject(int ownerActorNnum);

    /// <summary>
    /// PlayerTableから抽出対象のGameObjectをViewIDで検索して返す.
    /// PhotonPlayer.Find()の代替.
    /// ※自きゃらも検索対象.
    /// </summary>
    /// <param name="ownerActorNnum">抽出対象のPhotonView.viewID</param>
    /// <returns>なければnullと-1のタプル.</returns>
    public (GameObject obj, int sceneIndex, PhotonView view) FindUserObjectbyPhotonViewId(int viewId);

    /// <summary>
    /// PlayerTableから抽出対象のGameObjectを検索して返す
    /// UserIdで検索Ver.
    /// </summary>
    /// <param name="userId">一意のId.</param>
    /// <returns>なければnull.</returns>
    //  public GameObject ExtractingGameObject(string userId);

    /// <summary>
    /// この端末のPlayerと同じシーンにいる他のPlayerの参照を取得する.
    /// </summary>
    /// <returns></returns>
    // IReadOnlyCollection<GameObject> GetNowScenePlayerList();
    /// <summary>
    /// 指定したSceneと同じシーンにいる他のPlayerの参照を取得する.
    /// </summary>
    /// <returns></returns>
    List<GameObject> GetNowScenePlayerList(int targetSceneIndex);

    /// <summary>
    /// (この端末上に)生成された自身を含む全Playerの参照をすべて取得する.
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<(GameObject obj, int sceneIndex, Photon.Pun.PhotonView view)> GetAllUserData();

    /// <summary>
    /// この端末のPlayerと同じシーンにいる他のPlayerの参照を追加する.
    /// </summary>
 //   void AddNowScenePlayerList(GameObject otherUser);
    /// <summary>
    /// 指定したGameObjectをリストから取り除く.
    /// </summary>
    /// <param name="targetObj"></param>
 //   void RemoveNowScenePlayerList(GameObject targetObj);
    /// <summary>
    /// 今いるSceneの他PlayerのListをクリアする.
    /// </summary>
  //  void NowScenePlayerListClear();
    /// <summary>
    /// 指定したUser(のキャラ)がnowScenePlayerListもとい現在のUserと同じSceneに存在する場合はTrueを返す.
    /// </summary>
    /// <param name="targetUserId">一意のID.User固有.</param>
    /// <param name="myPlayerExistSceneIndex">この端末のUserの現在SceneIndex.</param>
    /// <returns></returns>
 //   bool IsContainNowScenePlayerList(string targetUserId, int myPlayerExistSceneIndex);


    /// <summary>
    ///  (この端末上に)生成された他の全Playerの参照をすべて取得する.
    ///  Tkey:GameObject   Tvalue:SceneIndex 
    ///  </summary>
    /// <returns></returns>
   // Dictionary<GameObject, int> GetAllPlayerMap();


    /// <summary>
    /// 指定したGameObjectのSceneIndexを取得する.
    /// </summary>
    /// <param name="targetObj"></param>
    /// <returns>取得出来ない場合は-1を返す.</returns>
  //  int GetTargetObjExistSceneIndex(GameObject targetObj);

    //  void SetSceneIndexForAllPlayerTable(GameObject obj, int sceneIndex);
    /// <summary>
    /// MasterOnly.指定したPhotonNetworkIDのSceneIndexを変更する.
    /// </summary>
    /// <param name="networkId"></param>
    /// <param name="sceneIndex"></param>
    //  void SetSceneIndexForAllPlayerIdTable(int networkId, int sceneIndex);

    
    void ClearAllUserTable(PUNController.Room room);

    //============リファ中のMock的な============
    UnusualStatusClass GetPlayerUnuStatus(int id);
    List<CharaStatus> PlayerStatusTable { get; }
    Dictionary<GameObject,int> AllplayerTable { get; }
    List< IEquipmentStatusAccess > EquipmentStatusAccessTable { get; }
    Dictionary<int, ISkillLevelClass> PlayerSkillLevelClassTable { get; }
}
