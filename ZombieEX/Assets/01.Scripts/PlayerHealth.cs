﻿using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity // PlayerHealth는 LivingEntity를 상속
{ 
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public AudioClip itemPickupClip; // 아이템 습득 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake() {
        // 사용할 컴포넌트를 가져오기
        playerAnimator = GetComponent<Animator>();
        playerAudioPlayer = GetComponent<AudioSource>();

        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        //사망한 경우 움직이거나 총을 쏠 수 없도록 비활성화 할 컴포넌트도 가져옴
    }

    //PlayerHealth 컴포넌트가 활성화될 때마다 체력 상태를 리셋하는 처리 구현
    protected override void OnEnable() //LivingEntity의 OnEnable()을 오버라이드로 구현
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();
        //체력 슬라이더 활성
        healthSlider.gameObject.SetActive(true);
        //체력 슬라이더의 최댓값을 기본 체력값으로 변경
        healthSlider.maxValue = startingHealth;
        //체력 슬라이더의 값을 현재 체력값으로 변경
        healthSlider.value = health;

        //플레이어 조작을 받는 컴포넌트 활성화
        playerMovement.enabled = true;
        playerShooter.enabled = true;

        //PlayerHealth의 OnEnable()메서드는 부활 기능을 염두에 둔 구현
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth) {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);
        //갱신된 체력으로 체력 슬라이더 갱신
        healthSlider.value = health;
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection) {

        //사망하지 않은 경우에만 효과음 재생
        //if (!dead) playerAudioPlayer.PlayOneShot(hitClip); //책방식
        if (dead) return; //쌤방식 //죽으면 리턴, 안죽었으면 하단 실행
        playerAudioPlayer.PlayOneShot(hitClip); //효과음 재생

        //리턴하지 않으면 하단까지 실행되어서 로직 꼬임 (죽어도 체력이 깎임)

        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection);
        //갱신된 체력을 체력 슬라이더에 반영
        healthSlider.value = health;
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();

        //체력 슬라이더 비활성화
        healthSlider.gameObject.SetActive(false);

        //사망음 재생
        playerAudioPlayer.PlayOneShot(deathClip);
        //애니메이터의 Die트리거 발동
        playerAnimator.SetTrigger("Die");

        //플레이어 조작을 받는 컴포넌트 비활성화
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        if(!dead)
        {
            //AmmoPack ammoPack = other.GetComponent<AmmoPack>();
            //if (ammoPack != null)
            //{ ammoPack.Use(this.gameObject); }

            //HealthPack healthPack = other.GetComponent<HealthPack>();
            //if (healthPack != null)
            //{ healthPack.Use(this.gameObject); }

            //↓ 인터페이스로 간결화

            IItem item = other.GetComponent<IItem>();//충돌한 상대방으로부터 IItem 컴포넌트 가져오기
            if (item != null) //가져왔으면
            {
                //Use 메서드 실행하여 아이템 사용
                item.Use(gameObject);
                // 아이템 습득 소리 재생
                playerAudioPlayer.PlayOneShot(itemPickupClip);
            }
        }
    }
}