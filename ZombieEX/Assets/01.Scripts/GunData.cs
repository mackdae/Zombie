using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "Gun Data")]
//유니티 에셋 생성 메뉴에서 GunData 타입을 생성할 수 있도록 특성(Attribute)을 클래스에 추가

public class GunData : ScriptableObject //스크립터블 오브젝트로 동작할 수 있도록 클래스 상속
{
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float damage = 25; // 공격력

    public int startAmmoRemain = 100; // 처음에 주어질 전체 탄약
    public int magCapacity = 25; // 탄창 용량

    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간
}