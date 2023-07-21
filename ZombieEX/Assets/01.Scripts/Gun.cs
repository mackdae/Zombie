using System.Collections;
using UnityEngine;

// 총을 구현
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는 데 사용할 타입을 선언
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄알집이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 탄알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    private LineRenderer bulletLineRenderer; // 탄알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer; // 총 소리 재생기

    public GunData gunData; // 총의 현재 데이터

    private float fireDistance = 50f; // 사정거리

    public int magAmmo; // 장전된 탄알
    public int ammoRemain = 100; // 소지한 탄알

    private float lastFireTime; // 총을 마지막으로 발사한 시점

    private void Awake() {
        // 사용할 컴포넌트의 참조 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // 사용할 점을 두 개로 변경
        bulletLineRenderer.positionCount = 2;
        // 라인 렌더러를 비활성화
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable() {
        // 총 상태 초기화

        // 전체 예비 탄알 양 초기화
        ammoRemain = gunData.startAmmoRemain;
        // 현재 탄창을 가득 채우기
        magAmmo = gunData.magCapacity;

        // 총의 현재 상태를 총을 쏠 준비가 된 상태로 변경
        state = State.Ready;
        // 마지막 발사 시점을 초기화
        lastFireTime = 0;
    }

    // 발사 시도
    public void Fire() {
        // 현재 상태가 발사 가능한 상태 && 마지막 발사 시점에서 gunData.timeBetFire 이상의 시간이 지남
        if (state == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            // 마지막 발사 시점갱신
            lastFireTime = Time.time;
            // 실제 발사 처리 실행
            Shot();
        }
    }

    // 실제 발사 처리
    private void Shot() {
        // 레이캐스트에 의한 충돌 정보를 저장하는 컨테이너
        RaycastHit hit;
        // 탄알이 맞은 곳을 저장할 변수
        Vector3 hitPosition = Vector3.zero;

        // 레이가 어떤 물체와 충돌한 경우
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        // 레이캐스트(Vector3 시작지점, Vector3 방향, out RaycastHit 충돌정보, float 사정거리) 충돌여부를 bool 반환
        // out 키워드는 메서드가 return 이외의 방법으로 추가 정보를 반환할 수 있게 만듦
        // out으로 입력된 변수는 메서드 내부에서 변경된 사항이 반영되어 되돌아옴
        {

            // 충돌한 상대방으로부터 IDamageable 오브젝트 가져오기
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            // 가져오는데 성공했다면
            if (target != null)
            {
                // 상대의 OnDamage 함수를 실행시켜서 대미지 주기
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }

            // 레이가 충돌한 위치 저장
            hitPosition = hit.point;
        }
        else
        {
            // 레이가 다른 물체와 충돌하지 않았다면
            //탄알이 최대 사정거리까지 날아갔을 때의 위치를 충돌 위치로 사용
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        // 발사 이펙트 재생
        StartCoroutine(ShotEffect(hitPosition));

        //magAmmo--; // 남은 탄알 수 -1
        //if (magAmmo <= 0) // 탄창에 남은 탄알이 없다면
        //{
        //    state = State.Empty; // 총의 현재 상태를 Empty로 갱신
        //}  

        // 위 코드 축약ver
        if (--magAmmo <= 0) state = State.Empty;
        //이렇게 쓰면 가독성이 떨어질 수 있으니 적절한 주석 필요
    }

    // 발사 이펙트와 소리를 재생하고 탄알 궤적을 그림
    private IEnumerator ShotEffect(Vector3 hitPosition) // 코루틴 메서드
    {
        //총구 화염 효과 재생
        muzzleFlashEffect.Play();
        //탄피 배출 효과 재생
        shellEjectEffect.Play();
        //총격소리 재생
        gunAudioPlayer.PlayOneShot(gunData.shotClip);
        //오디오 Play()는 하나만 재생, PlayOneShot()는 중첩 재생

        //선의 시작점은 총구의 위치
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        //선의 끝점은 입력으로 들어온 충돌 위치
        bulletLineRenderer.SetPosition(1, hitPosition);
        // 라인 렌더러를 활성화하여 탄알 궤적을 그림
        bulletLineRenderer.enabled = true;
        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);
        // 라인 렌더러를 비활성화하여 탄알 궤적을 지움
        bulletLineRenderer.enabled = false;
    }

    // 격발 / 발사 / 효과 메서드를 나눈 이유가 뭔지
    // 장전시도 / 장전처리 메서드를 나눈 이유가 뭔지 잘 모르겠다...

    // 재장전 시도
    public bool Reload() {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= gunData.magCapacity)
        {
            // 이미 재장전 중이거나 남은 탄알이 없거나 탄창에 탄알이 가득한 경우
            // 재장전 불가
            return false;
        }
        // 재장전 처리 시작
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() // 코루틴 메서드
    {
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;
        // 재장전 소리 재생
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);
        // 재장전 소요 시간 만큼 처리 쉬기
        yield return new WaitForSeconds(gunData.reloadTime);
        // 대기시간동안 state의 값이 Reloading이므로 Fire()나 Reload()가 실행되어도 작동하지 않음

        // 탄창에 채울 탄알 계산
        int ammoToFill = gunData.magCapacity - magAmmo;

        // 탄창에 채워야 할 탄알이 남은 탄알보다 많으면
        if (ammoRemain < ammoToFill)
        {
            // 채워야 할 탄알 수를 남은 탄알 수에 맞춰 줄임
            ammoToFill = ammoRemain;
        }

        // 탄창을 채움
        magAmmo += ammoToFill;
        // 남은 탄알에서 탄창에 채운만큼 탄알을 뺌
        ammoRemain -= ammoToFill;

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
    }

    // 코루틴(Coroutine) 메서드 대기시간을 가질 수 있음
    // 비동기 작업(Asynchronous operation)을 처리하기 위해 사용되는 메서드
    // 협력 루틴(Cooperative Routine)의 줄임말
    //코드 실행을 일시 중지하고 다른 작업을 수행한 후 다시 이어서 실행할 수 있는 기능을 제공합니다.
    //이를 통해 복잡한 비동기 동작을 간단하게 작성하고 제어할 수 있습니다.
    // IEnumerator 타입 반환, 처리를 일시대기할 곳에 yield 키워드 명시
    //IEnumerator는 반복자(iterator)를 나타내는 C# 인터페이스입니다.
    //이 인터페이스를 사용하여 코루틴이 실행되는 동안 일시 중지되었다가 재개될 때까지의 실행 상태를 유지합니다.

    // StartCoroutine()메서드로 실행
    // StartCoroutine(SomeCoroutine(입력값));
    //   코루틴 메서드를 실행하면서 그 반환값을 즉시 StartCoroutine에 입력
    // StartCoroutine("SomeCoroutine"); 
    //   나중에 StopCoroutine() 메서드를 사용해서 실행중인 코루틴을 도중에 종료 가능

    // yield return new WaitForSeconds(시간); 초단위로 쉬기
    // yield return null; 한프레임만 쉬기

}