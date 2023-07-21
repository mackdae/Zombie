using System; // event Action용
using UnityEngine;

// 생명체로서 동작할 게임 오브젝트들을 위한 뼈대를 제공
// 체력, 데미지 받아들이기, 사망 기능, 사망 이벤트를 제공
public class LivingEntity : MonoBehaviour, IDamageable { // 리빙엔티티가 생명체들의 최상위 클래스
    public float startingHealth = 100f; // 시작 체력
    public float health { get; protected set; } // 현재 체력
    public bool dead { get; protected set; } // 사망 상태
    public event Action onDeath; // 사망시 발동할 이벤트
    //Action는 입출력이 없는 메서드를 가리킬 수 있는 델리게이트(대리자)
    //델리게이트는 메서드를 값으로 할당받을 수 있는 타입

    //Action onClean; 델리게이트 선언
    //onClean += CleaningRoomA; 메서드 등록
    //onClean += CleaningRoomB(); 괄호를 붙이면 메서드 실행후 반환값 할당이 되어서 에러
    //onClean(); 델리게이트에 등록된 메서드 일괄 실행

    //이벤트는 연쇄 동작을 이끌어내는 사건
    //이벤트 자체는 어떤 일을 실행하지 않지만 이벤트를 구독하는 처리들이 연쇄적으로 실행됨
    //이벤트는 자신을 구독하는 메서드의 구현과 상관 없이 동작하므로 견고한 커플링 문제를 해소
    //이벤트를 구현하는 대표적인 방법: 델리게이트를 클래스 외부로 공개
    //델리게이트 타입의 변수는 event 키워드로 선언 가능
    //델리게이트를 event로 선언하면 클래스 외부에서 실행불가
    //(이벤트를 소유하지 않은 측에서 발동하는 것을 막을 수 있다)

    // 생명체가 활성화될때 상태를 리셋
    protected virtual void OnEnable()
    // protected 자신과 자식만 접근한정자
    // virtual 가상메서드, 자식의 재정의(오버라이드) 특성 부여
    // 오버라이드시 base.키워드를 사용하여 부모메서드 유지확장 가능
    {
        // 사망하지 않은 상태로 시작
        dead = false;
        // 체력을 시작 체력으로 초기화
        health = startingHealth;
    }

    // 데미지를 입는 기능
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    // IDamageable 인터페이스를 상속했으니 OnDamage를 반드시 구현해야 함
    {
        // 데미지만큼 체력 감소
        health -= damage;

        // 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    // 체력을 회복하는 기능
    public virtual void RestoreHealth(float newHealth) {
        if (dead)
        {
            // 이미 사망한 경우 체력을 회복할 수 없음
            return;
        }

        // 체력 추가
        health += newHealth;
    }

    // 사망 처리
    public virtual void Die() {
        // onDeath 이벤트에 등록된 메서드가 있다면 실행
        if (onDeath != null)
        {
            onDeath();
        }

        // 사망 상태를 참으로 변경
        dead = true;
    }
}