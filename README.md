# Unity Optional

Unity는 많은 연산이 `null`을 반환할 수 있도록 설계되어 있습니다. 이러한 설계는 당장에는 직관적이지만, 잠재적 `null`로 인해 번거로운 오류와 예외를 맞닥뜨릴 수 있습니다.

`Optional<T>`는 Unity의 `null` 처리 방식을 함수형 프로그래밍 관점에서 보완한 자료형입니다. 구조체라서 할당이 불필요하게 발생하지 않으며, 반복적인 `null` 검사 없이 안정적으로 연산을
조합하는 것이 가능합니다.

## 주요 기능

- 값이 있는 상태와 없는 상태를 `Optional<T>` 자료형 하나로 표현
  - `null` 값과 Unity 객체를 `Optional` 자료형으로 처리 가능
  - 잠재적 `null` 가능성을 배제할 수 있음
- 값이 있을 때만 변환하거나 조건을 적용하는 `Map`, `Filter` 등의 연산 지원
- `SerializableOptional<T>` 자료형으로 인스펙터에서 제어 가능

## 요구 사항

- Unity 6.0 (6000.0) 이상

## 설치

Unity Package Manager에서 **Add package from git URL...** 을 선택한 뒤 다음 주소를 입력합니다.

```text
https://github.com/qazxsw1240/Unity-Optional.git?path=Assets/Unity-Optional
```

## 시작하기

네임스페이스는 다음과 같습니다.

```csharp
using Unity.Utility.Functional;
```

확장 기능을 사용하려면 다음의 네임스페이스를 불러오면 됩니다.

```csharp
using Unity.Utility.Functional.Extensions;
```

## 생성 및 값 접근

기존의 자료형을 `Optional<T>`로 변환하려면 다음과 같은 메서드를 사용할 수 있습니다.

```csharp
Optional<int> option1 = Optional<int>.Empty;    // 빈 Optional 개체
Optional<int> option2 = Optional<int>.From(42); // 42가 값으로 있는 Optional 개체
Optional<int> option3 = 67.ToOptional();        // 67이 값으로 있는 Optional 개체
```

Unity의 `Object`는 소멸된 오브젝트에 대해 개별적인 `null` 처리를 수행하므로, 소멸된 오브젝트에 대해서 `Optional`을 적용하려면 `.ToUnityOptional()`을 사용해야 합니다.

```csharp
Object o = /** ... **/;
Object.Destory(o);      // 오브젝트가 소멸됨
o.ToOptional();         // 오브젝트가 소멸됐지만, null이 아니므로 값이 있다고 평가함
o.ToUnityOptionl();     // 오브젝트가 소멸된 것을 반영하여 값이 없다고 평가함
```

`Optional<T>`에서 값을 가져오려면 다음과 같은 메서드를 사용할 수 있습니다.

```csharp
// Option 1: Optional<T>.HasValue로 검사하고 Optional<T>.Value에 접근하기
// Optional<T>.HasValue로 검사하지 않고 Optional<T>.Value에 바로 접근할 경우
// Optional 개체가 빈 값일 때 오류가 발생함
Optional<int> option1 = /** ... **/;

if (option1.HasValue)
{
  int value = option1.Value;
}

// Option 2: Try-Get 패턴으로 값에 접근하기
// 가장 권장하는 방법으로, Optional<T>.TryGetValue(out T)로 값이 존재하는지 여부와
// Optional 내의 값을 동시에 확인할 수 있음
Optional<int> option2 = /** ... **/;

if (option2.TryGetValue(out int value))
{
  // ...
}
```

`??`와 같이 C#에서 제공하는 null 병합 연산자 (null-coalescing operators)의 개념을 `Optional<T>`에도 사용할 수 있습니다.

```csharp
// Or 메서드: Optional의 값이 비어 있으면 매개변수로 대체됩니다.
Optional<int>.From(42).Or(67.ToOptional()); // Optional(42)
Optional<int>.Empty.Or(67.ToOptional());    // Optional(67)

// OrElse 메서드: Optional의 값을 반환하되, 값이 비어 있으면 매개변수의 값으로 대체됩니다.
Optional<int>.From(42).OrElse(67); // 42
Optional<int>.Empty.OrElse(67);    // 67
```

## 값 가공 및 필터링

`Optional`의 기본 연산은 값이 있을 때에만 해당 콜백을 실행합니다. 값이 없는 경우에는 연산을 실행하지 않고 빈 `Optional`이 계속 유지됩니다.

연산의 결괏값이 `Optional<T>`인 경우, 이를 그대로 사용했을 때 `Optional` 안에 `Optional`이 중첩되는 형태 (`Optional<Optional<T>>`)가 될 수 있습니다. 이때
`FlatMerge`, `FlatFilter` 등의 `Flat` 연산을 사용하여 자료형의 중첩을 줄일 수 있습니다.

```csharp
// 예시 1: 현재 클라이언트의 플레이어 이름을 반환함
public static Optional<string> GetCurrentPlayerName() {
  Player? player = NetworkManager.CurrentPlayer;
  return player.ToOptional()    // Optional로 변환
    .Where(p => p.IsConnected)  // 플레이어가 연결되어 있는지 검사함
    .Map(p => p.Name);          // 플레이어가 연결되어 있다면 이름을 반환함
}
```

```csharp
// 예시 2: 플레이어가 소지한 아이템의 효과 접근하기

public class Item
{
  public Optional<ItemEffect> GetEffect()
  {
    // ...
  }
}

public class Inventory
{
  private readonly List<Optional<Item>> m_Items;
  private int m_CurrentSlot;

  public Inventory()
  {
    // ...
  }

  public Optional<Item> GetCurrentItem()
  {
    return m_Items[m_CurrentSlot];
  }

  public Optional<ItemEffect> GetCurrentItemEffect()
  {
    return GetCurrentItem()                 // Optional로 변환
      .FlatMap(item => item.GetEffect())    // 플레이어가 연결되어 있는지 검사함
  }
}
```

### 필터링

`Filter`, `FlatFilter`는 조건이 `true`(`FlatFilter`는 값이 존재하고 그 값이 `true`인 경우), `Optional`의 값을 유지합니다. 값이 조건에 맞지 않는다면 빈 값으로
만들어, 이후 연산이 추가로 작동하지 않도록 합니다.

`Map`은 콜백의 일반 반환값을 `Optional`로 감싸고, `FlatMap`은 콜백이 반환한 `Optional`을 그대로 이어갑니다. 두 입력을 함께 처리할 때는 `Merge` 또는 `FlatMerge`를
사용할 수 있습니다.
위 예시의 `FindPlayerName`은 `Optional<string>`을 반환하는 함수입니다.

### 가공

`Map`, `FlatMap`은 값이 존재할 경우, 값을 연산에 맞춰서 가공합니다. 값이 존재하지 않는다면 빈 `Optional` 개체를 생성합니다.

```csharp
Optional<string> s1 = Optional<int>.From(42).Map(i => i.ToString());    // Optional("42")
Optional<string> s2 = Optional<int>.Empty.Map(i => i.ToString());       // Optional.Emtpy
```

### 병합

`Merge`, `FlatMerge`는 값이 존재하는 두 `Optional`을 병합하여 새로운 `Optional`을 생성합니다.

```csharp
public static Optional<string> Repeat(Optional<string> content, Optional<int> count)
{
  return content.Merge(count, (s, i) => string.Concat(Enumerable.Repeat(s, i)));
}

Repeat(Optional<string>.Empty, Optional<int>.Empty);        // Optional.Empty
Repeat(Optional<string>.From("a"), Optional<int>.Empty);    // Optional.Empty
Repeat(Optional<string>.Empty, Optional<int>.From(5));      // Optional.Empty
Repeat(Optional<string>.From("a"), Optional<int>.From(5));  // Optional("aaaaa")
```

## 확장 기능

일부 기능은 `Optional`을 활용한 확장 메서드로 사용할 수 있습니다.

일반적인 Unity 기능은 `null`을 반환할 수 있기에 다음과 같이 사용하게 됩니다.

```csharp
using UnityEngine;

using Unity.Utility.Functional;

public sealed class PlayerNameLabel : MonoBehaviour
{
  private void Start()
  {
    string playerName = GameObject.Find("Player")
      .ToUnityOptional()
      .Map(player => player.name)
      .OrElse("Player not found");

    Debug.Log(playerName);
  }
}
```

이런 경우 `GameObjectOptional` 클래스를 활용하여 코드를 더 간결하게 만들 수 있습니다.

```csharp
using UnityEngine;

using Unity.Utility.Functional;
using Unity.Utility.Functional.Extensions;

public sealed class PlayerNameLabel : MonoBehaviour
{
  private void Start()
  {
    string playerName = GameObjectOptional.Find("Player")
      .Map(player => player.name)
      .OrElse("Player not found");

    Debug.Log(playerName);
  }
}
```

이밖에 `GameObject.GetComponentOptional<T>`, `GameObject.GetComponentInChildrenOptional<T>` 등 `Optional` 접미사가 붙은 확장 기능으로
Unity 기본 기능을 `Optional`로 접근하여 처리할 수 있습니다.

## Unity 인스펙터에서 값 관리하기

`Optional<T>`를 인스펙터에서 편집하려면 `SerializableOptional<T>`를 사용합니다. `Build()`는 이를 런타임에서 사용하는 `Optional<T>`로 변환합니다.

```csharp
using UnityEngine;
using Unity.Utility.Functional;

public sealed class PlayerSettings : MonoBehaviour
{
  [SerializeField]
  private SerializableOptional<int> m_StartingLives =
    SerializableOptional<int>.From(3);

  private void Start()
  {
    Optional<int> startingLives = m_StartingLives.Build();
    if (startingLives.TryGetValue(out int value))
    {
      Debug.Log($"Starting lives: {value}");
    }
  }
}
```

인스펙터에서 **Contain Value**와 **Remove Value**를 눌러 값의 유무를 바꿀 수 있습니다. 코드에서는 `HasValue`를 설정하거나 `From(value)`와 `Empty`로 인스턴스를
만들 수 있습니다. `Optional<T>`에서 `SerializableOptional<T>`로 암시적 변환도 가능합니다.

```csharp
Optional<int> runtimeValue = Optional<int>.From(5);
SerializableOptional<int> serializedValue = runtimeValue;
```

> `Object` 필드를 참조하는 `SerializableOptional<T>` 자료형의 경우, 직렬화된 상태에서 값이 있다고 표시되어도 `Object` 필드의 값이 `null`이면 `Build()`는 빈
> `Optional<T>`를 반환합니다.
