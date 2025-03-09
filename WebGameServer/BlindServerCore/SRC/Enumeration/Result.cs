namespace BlindServerCore;

public enum Result
{
    Error_Unknown = 0,
    Error_Internal,
    Error_NoService,
    Error_EmptyPlayer,
    Error_LoadRequiredData,
    Error_UserNotFound,
    Error_UnknownChannelType,
    Error_TimeOut,
    Error_UnknownUseItemType,

    Success = 100,
    /// <summary>
    /// 잘못된 파라미터
    /// </summary>
    Error_InvalidParameter = 1000,
    /// <summary>
    /// 캐싱된 계정 정보를 찾을수 없음 다시 로그인 필요.
    /// </summary>
    Error_CacheAccountNotFound,
    /// <summary>
    /// 계정 및 패스워드등 문제 있음
    /// </summary>
    Error_InvalidIdOrPass,
    /// <summary>
    /// 외부 인증 실패
    /// </summary>
    Error_ThridPartyValidateFail,
    /// <summary>
    /// 확인 불가능한 인증키
    /// </summary>
    Error_NotFoundAuthKey,
    /// <summary>
    /// 계정 블럭중
    /// </summary>
    Error_Block,
    /// <summary>
    /// 현재 이용 불가능 서버임으로 서버 목록을 갱신해 주세요.
    /// </summary>
    Error_NotAvailableWorld,
    /// <summary>
    /// 재접속 할수 없는 상태임 로그인으로 돌아감
    /// </summary>
    Error_ReonnectFail,
    /// <summary>
    /// 중복 접속 시도
    /// </summary>
    Error_DuplicateConnect,
    /// <summary>
    /// 네이밍 중복
    /// </summary>
    Error_DuplicateName,
    /// <summary>
    /// 테이블 데이터를 찾을수 없음
    /// </summary>
    Error_TableNotFound,
    /// <summary>
    /// 호스트 플레이어가 아님
    /// </summary>
    Error_IsNotHost,
    /// <summary>
    /// 해당 타입의 인벤토리 슬롯이 없음
    /// </summary>
    Error_UndefinedInventorySlotType,
    /// <summary>
    /// 아이템 수량만큼 들고 있지 않음
    /// </summary>
    Error_NotEnoughItemCount,
    /// <summary>
    /// 아이템을 찾을수 없음
    /// </summary>
    Error_ItemNotFound,
    /// <summary>
    /// 인벤토리 최대 확장슬롯임
    /// </summary>
    Error_InventoryMaxSlotOver,
    /// <summary>
    /// 인벤토리 공간 부족
    /// </summary>
    Error_InventoryFull,
    /// <summary>
    /// 인벤토리 무게 초과
    /// </summary>
    Error_InventoryOverWeight,
    /// <summary>
    /// 레벨이 낮음
    /// </summary>
    Error_LowLevel,
    /// <summary>
    /// 아이템 장착 중 삭제 불가
    /// </summary>
    Error_EquipedCannotDelete,
    /// <summary>
    /// 아이템의 기간이 지남
    /// </summary>
    Error_ItemExpired,
    /// <summary>
    /// 허용불가능한 문자가 포함됨
    /// </summary>
    Error_NotAllowText,
    /// <summary>
    /// 아이템이 잠금 상태임
    /// </summary>
    Error_ItemLockState,
    /// <summary>
    /// 점검 진행중
    /// </summary>
    Error_InspectState,
    /// <summary>
    /// 장착중인 장비가 있음
    /// </summary>
    Error_EquipmentExist,
    /// <summary>
    /// 프리셋에 장착되어 있음
    /// </summary>
    Error_PresetExist,
    /// <summary>
    /// 테이블과의 조건이 맞지 않음
    /// </summary>
    Error_NotMatchCondition,
    /// <summary>
    /// 존재하지 않는 퀘스트임
    /// </summary>
    Error_NotFoundQuest,
    /// <summary>
    /// 퀘스트 완료 상태가 아님
    /// </summary>
    Error_NotCompleteQuest,
    /// <summary>
    /// 이미 완료한 퀘스트임
    /// </summary>
    Error_AlreadyCompleteQuest,
    /// <summary>
    /// 퀘스트 수락 조건 미충분
    /// </summary>
    Error_NotEnoughQuestAcceptCondition,
    /// <summary>
    /// 퀘스트 수락 최대 개수 오버
    /// </summary>
    Error_QuestAcceptLimitOver,
    /// <summary>
    /// 재화 부족
    /// </summary>
    Error_NotEnoughGoods,
    /// <summary>
    /// 소유권이 없는 상태
    /// </summary>
    Error_NotOwnership,
    /// <summary>
    /// 상점이 닫힌 상태임
    /// </summary>
    Error_StoreClose,
    /// <summary>
    /// 상점 판매 종료된 상품임
    /// </summary>
    Error_StoreSoldOut,
    /// <summary>
    /// 상점에 오퍼한 상품의 수량 부족
    /// </summary>
    Error_StoreProductStockNotEnough,
    /// <summary>
    /// 우편 또는 쪽지 받는 사람을 찾을 수 없음
    /// </summary>
    Error_NotFoundReceiver,
    /// <summary>
    /// 우편 또는 쪽지의 유효기간이 지남
    /// </summary>
    Error_MailExpired,
    /// <summary>
    /// 우편의 보상이 있어, 삭제할 수 없음
    /// </summary>
    Error_MailHasItem,
    /// <summary>
    /// 상대 인벤토리 정리 필요
    /// </summary>
    Error_NeedToOrganizeTargetInventory,
    /// <summary>
    /// 상대 유저를 찾을 수 없음
    /// </summary>
    Error_TargetUserNotFound,
    /// <summary>
    /// 이미 거래 진행 중
    /// </summary>
    Error_AlreadyTrading,
    /// <summary>
    /// 거래 취소
    /// </summary>
    Error_CancelTrade,
    /// <summary>
    /// 거래 불가능한 거리
    /// </summary>
    Error_NotTradableDistance,
    /// <summary>
    /// 귀속된 아이템
    /// </summary>
    Error_ItemVestedState,
    /// <summary>
    /// 거래 슬롯 초과
    /// </summary>
    Error_TradeSlotMax,
    /// <summary>
    /// 이미 거래 중인 아이템
    /// </summary>
    Error_AlreadyTradingItem,
    /// <summary>
    /// 유효하지 않은 거래 요청
    /// </summary>
    Error_InvalidTrade,
    /// <summary>
    /// 상점 오픈이 불가능한 지역
    /// </summary>
    Error_ImpossibleArea,
    /// <summary>
    /// 이미 상점이 열려 있는 상태
    /// </summary>
    Error_AlreadyOpenShop,
    /// <summary>
    /// 존재하지 않는 인스턴스 입니다.
    /// </summary>
    Error_NotFoundInstance,
    /// <summary>
    /// 프리셋 제한됨
    /// </summary>
    Error_PresetLimit,
    /// <summary>
    /// 아직 정해진 시간이 되지 않았음.
    /// </summary>
    Error_NotTimeYet,
    /// <summary>
    /// 서버 이동중 해쉬 불일치
    /// </summary>
    Error_MoveMapInvalidHash,
    /// <summary>
    /// 플레이어가 존에 속해 있지 않음. (로그아웃)
    /// </summary>
    Error_PlayerNotEnterMap,
    /// <summary>
    /// 플레이어 오브젝트가 존재하지 않음. (로그아웃)
    /// </summary>
    Error_NotFoundPlayerObject,
    /// <summary>
    /// 약관 동의가 필요 합니다.
    /// </summary>
    Error_FirstAgreePolicy,
    /// <summary>
    /// 존재하지 않는 플레이어
    /// </summary>
    Error_PlayerNotFound,
    /// <summary>
    /// 요청 제한이 걸린 상태임
    /// </summary>
    Error_RequestLimitInterval,
    /// <summary>
    /// 획득하지 못한 칭호
    /// </summary>
    Error_TextTitleNotAcquired,
    /// <summary>
    /// 최소 입장 제한 시간을 넘지 못함
    /// </summary>
    Error_NotEnoughEntranceTime,
    /// <summary>
    /// 플레이어는 접속 상태가 아님.
    /// </summary>
    Error_PlayerOffline,
    /// <summary>
    /// 진입이 허용되지 않은 유저
    /// </summary>
    Error_NotAllowEnterUser,
    /// <summary>
    /// 아이템 사용조건 충족하지 않음.
    /// </summary>
    Error_NotAvailableItemUse,
    /// <summary>
    /// 길드를 찾을 수 없음
    /// </summary>
    Error_NotFoundGuild,
    /// <summary>
    /// 길드 멤버가 아님
    /// </summary>
    Error_NotGuildMember,
    /// <summary>
    /// 최대 제한 길이 초과
    /// </summary>
    Error_OverTextLimitLength,
    /// <summary>
    /// 길드의 재가입 대기 시간을 넘지 못함
    /// </summary>
    Error_NotEnoughGuildJoinWaitTime,
    /// <summary>
    /// 길드 맴버 최대
    /// </summary>
    Error_MaxGuildMember,
    /// <summary>
    /// 이미 길드에 가입 되어있는 상태
    /// </summary>
    Error_AlreadyGuildMember,
    /// <summary>
    /// 이미 길드에 가입 신청 되어있는 상태
    /// </summary>
    Error_AlreadyGuildJoindMember,
    /// <summary>
    /// 이미 길드에 초대 신청 되어있는 상태
    /// </summary>
    Error_AlreadyGuildInvitedMember,
    /// <summary>
    /// 길드에 가입할 수 없는 모집 형태
    /// </summary>
    Error_NotAllowGuildRegisterType,
    /// <summary>
    /// 유저가 신청할 수 있는 길드 가입 신청 횟수 초과
    /// </summary>
    Error_OverUserToGuildJoinCount,
    /// <summary>
    /// 길드가 받을 수 있는 유저 가입 신청 횟수 초과
    /// </summary>
    Error_OverGuildFromUserJoinCount,
    /// <summary>
    /// 유저가 받을 수 있는 길드 초대 신청 횟수 초과
    /// </summary>
    Error_OverUserFromGuildInviteCount,
    /// <summary>
    /// 길드가 초대할 수 있는 유저 초대 횟수 초과
    /// </summary>
    Error_OverGuildToUserInviteCount,
    /// <summary>
    /// 길드 가입 신청 목록에 존재하지 않는 대상
    /// </summary>
    Error_NotGuildJoinMember,
    /// <summary>
    /// 길드 초대 신청 목록에 존재하지 않는 대상
    /// </summary>
    Error_NotGuildInviteMember,
    /// <summary>
    /// 이미 길드에 출석 되어있는 상태
    /// </summary>
    Error_AlreadyGuildAttend,
    /// <summary>
    /// 기부 가능 횟수 초과
    /// </summary>
    Error_OverGuildDonateCount,
    /// <summary>
    /// 이미 해당 길드 기부 보상을 받은 상태
    /// </summary>
    Error_AlreadyGuildDonateReward,
    /// <summary>
    /// 길드 기부 카운트 부족
    /// </summary>
    Error_NotEnoughGuildDonateTotalCount,
    /// <summary>
    /// 길드 가입 후 바로 출석 및 기부 행위 불가
    /// </summary>
    Error_NotAllowNewGuildMemberAttendDonate,
    /// <summary>
    /// 길드 권한이 없음
    /// </summary>
    Error_NotHaveGuildPermission,
    /// <summary>
    /// 길드 마스터가 아닌 경우
    /// </summary>
    Error_NotGuildMaster,
    /// <summary>
    /// 만료된 길드 정보
    /// </summary>
    Error_ExpireGuildInfo,
    // Freind Error Code
    /// <summary>
    /// 자기 자신을 타겟으로 할 수 없음
    /// </summary>
    Error_NotTargetMyself,
    /// <summary>
    /// 친구 목록 초과
    /// </summary>
    Error_OverFriendListCount,
    /// <summary>
    /// 상대방 친구 목록 초과
    /// </summary>
    Error_OverTargetFriendListCount,
    /// <summary>
    /// 친구 요청 목록 초과
    /// </summary>
    Error_OverFriendRequestListCount,
    /// <summary>
    /// 상대방 친구 대기 목록 초과
    /// </summary>
    Error_OverTargetFriendWaitingListCount,
    /// <summary>
    /// 차단 목록 초과
    /// </summary>
    Error_OverBlockListCount,
    /// <summary>
    /// 이미 친구인 상태
    /// </summary>
    Error_AlreadyFriend,
    /// <summary>
    /// 차단한 유저
    /// </summary>
    Error_BlockUser,
    /// <summary>
    /// 이미 친구 요청이 온 상태
    /// </summary>
    Error_AlreadyFriendRequest,
    /// <summary>
    /// 친구 요청 처리 중
    /// </summary>
    Error_ProcessingFriendRequest,
    /// <summary>
    /// 친구 요청 대기 중이 아님
    /// </summary>
    Error_NotWaitingFriendRequest,
    /// <summary>
    /// 메일이 존재하지 않습니다.
    /// </summary>
    Error_MailNotFound,
};
