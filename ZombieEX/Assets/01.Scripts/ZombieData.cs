using UnityEngine;

// 좀비 생성시 사용할 셋업 데이터
[CreateAssetMenu(menuName = "Scriptable/ZombieData", fileName = "Zombie Data")]
// 프로젝트 창에서 에셋 메뉴에서 생성할 수 있게 특성 추가
public class ZombieData : ScriptableObject //좀비데이타는 좀비 생성시 사용할 수치를 모아두는 데이터 컨테이너
{
    public float health = 100f; // 체력
    public float damage = 20f; // 공격력
    public float speed = 2f; // 이동 속도
    public Color skinColor = Color.white; // 피부색
}
