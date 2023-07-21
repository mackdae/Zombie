using System.Collections.Generic;
using UnityEngine;

// 좀비 게임 오브젝트를 주기적으로 생성
public class ZombieSpawner : MonoBehaviour {
    public Zombie zombiePrefab; // 생성할 좀비 원본 프리팹

    public ZombieData[] zombieDatas; // 사용할 좀비 셋업 데이터들 배열
    public Transform[] spawnPoints; // 좀비 AI를 소환할 위치들 배열

    private List<Zombie> zombies = new List<Zombie>(); // 생성된 좀비들을 담는 리스트
    private int wave; // 현재 웨이브

    // 배열은 처음 정한 길이를 도중에 변경할 수 없음
    // (새로운 길이의 배열을 재할당할 수 있지만 기존 정보는 파괴됨)
    // 리스트는 저장 공간의 크기가 자유롭게 자동으로 변함
    //names.Add("Lee"); 리스트에 추가
    //names.Remove("Kim"); 리스트에서 제거
    //string name = names[0] 개별 오브젝트는 배열처럼 리스트명[번호]로 접근
    //int numberOfNames = names.Count; 등록 오브젝트 갯수에 접근

    private void Update() {
        // 게임 오버 상태일때는 생성하지 않음
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // 좀비를 모두 물리친 경우 다음 스폰 실행
        if (zombies.Count <= 0)
        {
            SpawnWave();
        }

        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        // 현재 웨이브와 남은 적 수 표시
        UIManager.instance.UpdateWaveText(wave, zombies.Count);
        // UI매니저 싱글턴 인스턴스를 사용
    }

    // 현재 웨이브에 맞춰 좀비들을 생성
    private void SpawnWave() {
        //웨이브 1 증가
        wave++;

        //좀비생성량은 현재 웨이브*1.5를 반올림한 수
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        //spawnCount만큼 좀비 생성
        for (int i = 0; i < spawnCount; i++)
        {
            //좀비 생성 메서드 실행
            CreateZombie();
        }

    }

    // 좀비를 생성하고 생성한 좀비에게 추적할 대상을 할당
    private void CreateZombie() {

        //사용할 좀비 데이터 랜덤으로 결정
        ZombieData zombieData = zombieDatas[Random.Range(0, zombieDatas.Length)];
        //생성할 위치를 랜덤으로 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        //좀비 프리팹으로부터 좀비 생성
        Zombie zombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
        //생성한 좀비의 능력치 설정
        zombie.Setup(zombieData);
        //생성된 좀비를 리스트에 추가
        zombies.Add(zombie);

        //좀비의 onDeath 이벤트에 익명 메서드 등록
        //사망한 좀비를 리스트에서 제거
        zombie.onDeath += () => zombies.Remove(zombie);
        //사망한 좀비를 10초 뒤에 파괴
        zombie.onDeath += () => Destroy(zombie.gameObject, 10f);
        //좀비 사망 시 점수 상승
        zombie.onDeath += () => GameManager.instance.AddScore(100);
        // 익명함수 람다식 람다표현식 ( 매개변수 ) => { 함수내용 }

        //private void OnZombieDeath()
        //{ zombies.Remove(zombie); }
        // 메서드를 만들어서 이벤트구독에 추가하는 과정을 일회용 메서드로 대체 축약
        //zombie.onDeath += OnZombieDeath;
    }
}