# unitiy-tcp-PcOnOff-MagicPackect-Server

유니티, C# UDP, TCP 통신을 이용한 클라이언트 PC ON/OFF 구현

-기능
1. PC ON 기능 -> UDP로 구현 -> 서버에서 클라이언트로 매직패킷 전송 // 현재 저장소 
2. PC OFF 기능 -> TCP로 구현 -> 서버와 클라이언트가 송수신 (서버 송신 :브로드캐스트)

-파일
1. 서버 (MagicPackect) // 현재 저장소 
2. 클라이언트(Client) 


pc off 기능은 cmd-shutdown 을 시키도록 구현했습니다. 
유니티 버젼은 2021.3.14f1 입니다.
기본 값 ip, port, mac address 직접 다 고치셔야됩니다!