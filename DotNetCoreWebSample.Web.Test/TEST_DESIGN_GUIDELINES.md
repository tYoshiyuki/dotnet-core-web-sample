# テスト設計指針

## 1. テスト戦略

### 1.1 テストピラミッド
- **単体テスト（Unit Tests）**: 70% - サービス層・リポジトリ層
- **統合テスト（Integration Tests）**: 20% - コントローラー層
- **エンドツーエンドテスト（E2E Tests）**: 10% - 全体フロー

### 1.2 カバレッジ目標
- **ラインカバレッジ**: 80%以上
- **ブランチカバレッジ**: 75%以上
- **メソッドカバレッジ**: 90%以上

## 2. テスト設計原則

### 2.1 AAA パターン（Arrange-Act-Assert）
すべてのテストケースは以下の構造に従う：
```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange: テストデータの準備、モックの設定
    // Act: テスト対象メソッドの実行
    // Assert: 結果の検証
}
```

### 2.2 テスト命名規則
- 形式: `メソッド名_シナリオ_期待される動作`
- 例: `Get_存在しないID_NotFoundを返す`
- 正常系: `Create_正常系_レコードが作成される`
- 異常系: `Create_Nullが渡される_ArgumentNullExceptionが発生する`

### 2.3 テスト分類
- **正常系テスト**: 期待通りの動作を確認
- **異常系テスト**: エラーケース、エッジケースを確認
- **境界値テスト**: 境界値での動作を確認
- **null値テスト**: nullが渡された場合の動作を確認

## 3. レイヤー別テスト設計

### 3.1 サービス層（Service Layer）

#### 3.1.1 ToDoService テストケース

**既存テスト:**
- ✅ `Get_正常系` - IDで取得（正常系）
- ✅ `GetList_正常系` - リスト取得（正常系）

**追加すべきテストケース:**

1. **Get メソッド**
   - `Get_存在しないID_Nullを返す` - 異常系
   - `Get_負のID_例外が発生する` - 異常系
   - `Get_ゼロID_例外が発生する` - 異常系

2. **GetList メソッド**
   - `GetList_空のリスト_空のリストを返す` - 異常系
   - `GetList_リポジトリで例外発生_例外が伝播する` - 異常系

3. **Create メソッド**
   - `Create_正常系_レコードが作成される` - 正常系
   - `Create_Nullが渡される_ArgumentNullExceptionが発生する` - 異常系
   - `Create_リポジトリで例外発生_例外が伝播する` - 異常系
   - `Create_正常系_SaveAsyncが呼ばれる` - 検証

4. **Edit メソッド**
   - `Edit_正常系_レコードが更新される` - 正常系
   - `Edit_Nullが渡される_ArgumentNullExceptionが発生する` - 異常系
   - `Edit_リポジトリで例外発生_例外が伝播する` - 異常系
   - `Edit_正常系_SaveAsyncが呼ばれる` - 検証

5. **Delete メソッド**
   - `Delete_正常系_レコードが削除される` - 正常系
   - `Delete_Nullが渡される_ArgumentNullExceptionが発生する` - 異常系
   - `Delete_リポジトリで例外発生_例外が伝播する` - 異常系
   - `Delete_正常系_SaveAsyncが呼ばれる` - 検証

6. **Exists メソッド**
   - `Exists_存在するID_Trueを返す` - 正常系
   - `Exists_存在しないID_Falseを返す` - 正常系
   - `Exists_負のID_Falseを返す` - 異常系
   - `Exists_ゼロID_Falseを返す` - 異常系

### 3.2 リポジトリ層（Repository Layer）

#### 3.2.1 TodoRepository テストケース

**既存テスト:**
- ✅ `Get_正常系` - 全件取得
- ✅ `Get_条件有_正常系` - 条件付き取得
- ✅ `Add_正常系` - 追加
- ✅ `Find_正常系` - ID検索

**追加すべきテストケース:**

1. **Update メソッド**
   - `Update_正常系_レコードが更新される` - 正常系
   - `Update_Nullが渡される_ArgumentNullExceptionが発生する` - 異常系

2. **Remove メソッド**
   - `Remove_正常系_レコードが削除される` - 正常系
   - `Remove_Nullが渡される_ArgumentNullExceptionが発生する` - 異常系

3. **Save メソッド**
   - `Save_正常系_変更が保存される` - 正常系
   - `Save_保存失敗_例外が発生する` - 異常系

4. **SaveAsync メソッド**
   - `SaveAsync_正常系_変更が保存される` - 正常系
   - `SaveAsync_保存失敗_例外が発生する` - 異常系
   - `SaveAsync_正常系_影響を受けた行数が返される` - 正常系

5. **FindAsync メソッド**
   - `FindAsync_正常系_レコードが取得される` - 正常系
   - `FindAsync_存在しないID_Nullを返す` - 異常系
   - `FindAsync_負のID_Nullを返す` - 異常系

6. **GetAsync メソッド（条件なし）**
   - `GetAsync_正常系_全件取得される` - 正常系
   - `GetAsync_空のテーブル_空のリストを返す` - 異常系

7. **GetAsync メソッド（条件あり）**
   - `GetAsync_条件有_正常系_条件に一致するレコードが取得される` - 正常系
   - `GetAsync_条件有_一致なし_空のリストを返す` - 異常系
   - `GetAsync_条件有_Nullが渡される_ArgumentNullExceptionが発生する` - 異常系

8. **GetCount メソッド**
   - `GetCount_正常系_レコード数が返される` - 正常系
   - `GetCount_空のテーブル_ゼロを返す` - 異常系

9. **GetCountAsync メソッド**
   - `GetCountAsync_正常系_レコード数が返される` - 正常系
   - `GetCountAsync_空のテーブル_ゼロを返す` - 異常系

### 3.3 コントローラー層（Controller Layer）

#### 3.3.1 TodoController テストケース

**新規作成が必要:**

1. **Index アクション**
   - `Index_正常系_ViewResultを返す` - 正常系
   - `Index_正常系_GetListが呼ばれる` - 検証
   - `Index_サービスで例外発生_例外が伝播する` - 異常系

2. **Details アクション**
   - `Details_正常系_ViewResultを返す` - 正常系
   - `Details_Nullが渡される_NotFoundを返す` - 異常系
   - `Details_存在しないID_NotFoundを返す` - 異常系
   - `Details_正常系_Getが呼ばれる` - 検証

3. **Create アクション（GET）**
   - `Create_GET_正常系_ViewResultを返す` - 正常系
   - `Create_GET_正常系_CreatedDateが設定される` - 正常系

4. **Create アクション（POST）**
   - `Create_POST_正常系_リダイレクトされる` - 正常系
   - `Create_POST_正常系_Createが呼ばれる` - 検証
   - `Create_POST_ModelStateが無効_ViewResultを返す` - 異常系
   - `Create_POST_サービスで例外発生_例外が伝播する` - 異常系

5. **Edit アクション（GET）**
   - `Edit_GET_正常系_ViewResultを返す` - 正常系
   - `Edit_GET_Nullが渡される_NotFoundを返す` - 異常系
   - `Edit_GET_存在しないID_NotFoundを返す` - 異常系
   - `Edit_GET_正常系_Getが呼ばれる` - 検証

6. **Edit アクション（POST）**
   - `Edit_POST_正常系_リダイレクトされる` - 正常系
   - `Edit_POST_ID不一致_NotFoundを返す` - 異常系
   - `Edit_POST_ModelStateが無効_ViewResultを返す` - 異常系
   - `Edit_POST_正常系_Editが呼ばれる` - 検証
   - `Edit_POST_DbUpdateConcurrencyException_存在する場合_例外が再スローされる` - 異常系
   - `Edit_POST_DbUpdateConcurrencyException_存在しない場合_NotFoundを返す` - 異常系

7. **Delete アクション（GET）**
   - `Delete_GET_正常系_ViewResultを返す` - 正常系
   - `Delete_GET_Nullが渡される_NotFoundを返す` - 異常系
   - `Delete_GET_存在しないID_NotFoundを返す` - 異常系
   - `Delete_GET_正常系_Getが呼ばれる` - 検証

8. **DeleteConfirmed アクション（POST）**
   - `DeleteConfirmed_正常系_リダイレクトされる` - 正常系
   - `DeleteConfirmed_正常系_Deleteが呼ばれる` - 検証
   - `DeleteConfirmed_サービスで例外発生_例外が伝播する` - 異常系

9. **TodoExists メソッド（プライベート）**
   - コントローラーのテストを通じて間接的にテスト

## 4. モック・スタブの使用方針

### 4.1 モックの使用
- **サービス層**: リポジトリをモック化
- **コントローラー層**: サービスをモック化
- **リポジトリ層**: DbContextをモック化（既存実装を参考）

### 4.2 モック検証
- メソッドが呼ばれたことを検証（`Verify`）
- 呼び出し回数を検証（`Times.Once()`, `Times.Never()`）
- パラメータを検証（`It.Is<T>`）

### 4.3 テストデータの管理
- テストデータはテストクラスのコンストラクタで初期化
- 各テストケースで必要に応じてデータを拡張
- テストデータは不変（Immutable）として扱う

## 5. エラーハンドリングテスト

### 5.1 例外処理テスト
- **ArgumentNullException**: nullパラメータ
- **ArgumentException**: 不正なパラメータ
- **NotFoundException**: リソースが見つからない
- **DbUpdateConcurrencyException**: 同時更新エラー

### 5.2 例外検証方法
```csharp
// 例外が発生することを検証
await Assert.ThrowsAsync<ArgumentNullException>(() => _service.Create(null));

// 例外メッセージを検証
var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _service.Create(null));
Assert.Contains("todo", exception.Message);
```

## 6. 非同期メソッドのテスト

### 6.1 非同期テストの記述
- `async Task` を使用
- `await` を適切に使用
- 非同期メソッドの完了を待機

### 6.2 非同期テストの検証
- 非同期メソッドの戻り値を検証
- 非同期メソッドが呼ばれたことを検証
- タイムアウトの設定（必要に応じて）

## 7. テストの独立性

### 7.1 テストの分離
- 各テストは独立して実行可能
- テスト間で状態を共有しない
- テストの実行順序に依存しない

### 7.2 テストデータの分離
- 各テストで独自のテストデータを使用
- テストデータの変更が他のテストに影響しない

## 8. パフォーマンステスト

### 8.1 パフォーマンス考慮事項
- 大量データでのテスト（必要に応じて）
- メモリリークの検証（必要に応じて）
- 実行時間の測定（必要に応じて）

## 9. テストの保守性

### 9.1 コードの重複排除
- 共通のテストデータはコンストラクタで初期化
- 共通のアサーションはヘルパーメソッド化
- テストフィクスチャの活用（必要に応じて）

### 9.2 テストの可読性
- テストメソッド名は明確に
- テストコードにコメントを追加（必要に応じて）
- テストデータの意図を明確に

## 10. 実装優先順位

### 優先度1（高）: サービス層の異常系テスト
- ToDoService の Create, Edit, Delete, Exists メソッド
- 異常系テスト（nullチェック、例外処理）

### 優先度2（中）: リポジトリ層の残りのメソッド
- Update, Remove, Save, SaveAsync
- GetCount, GetCountAsync
- FindAsync, GetAsync

### 優先度3（中）: コントローラー層のテスト
- TodoController の全アクション
- 正常系・異常系の両方をカバー

### 優先度4（低）: エッジケース・境界値テスト
- 境界値テスト
- パフォーマンステスト（必要に応じて）

## 11. テスト実行とカバレッジ測定

### 11.1 テスト実行
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### 11.2 カバレッジレポート
- カバレッジレポートの生成
- カバレッジの分析
- カバレッジが低い部分の特定

## 12. 継続的改善

### 12.1 テストレビュー
- テストコードのレビュー
- テストカバレッジの確認
- テストの品質向上

### 12.2 リファクタリング
- テストコードのリファクタリング
- テストの最適化
- テストの追加・削除

