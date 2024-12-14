<?php

return [

    /*
    |--------------------------------------------------------------------------
    | Cross-Origin Resource Sharing (CORS) Configuration
    |--------------------------------------------------------------------------
    |
    | Here you may configure your settings for cross-origin resource sharing
    | or "CORS". This determines what cross-origin operations may execute
    | in web browsers. You are free to adjust these settings as needed.
    |
    | To learn more: https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
    |
    */

    'paths' => ['api/*'], // CORSを適用するURLパス。APIエンドポイントに適用。

    'allowed_methods' => ['http://localhost'], // 許可するHTTPメソッド。'*' はすべてを許可。

    'allowed_origins' => ['http://localhost'], // 許可するオリジン（リクエスト元のドメイン）。'*' はすべてを許可。

    'allowed_origins_patterns' => [], // 特定のパターンに基づいてオリジンを許可する場合に使用。

    'allowed_headers' => ['http://localhost'], // 許可するHTTPヘッダー。'*' はすべてを許可。

    'exposed_headers' => [], // クライアントに公開するヘッダーを指定（必要なら設定）。

    'max_age' => 0, // ブラウザのプリフライトリクエストをキャッシュする秒数。0で無効。

    'supports_credentials' => false, // 認証情報（クッキーなど）を許可するかどうか。
];
