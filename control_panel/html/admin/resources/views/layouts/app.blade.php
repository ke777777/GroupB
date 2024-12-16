<!DOCTYPE html>
<html lang="{{ str_replace('_', '-', app()->getLocale()) }}">
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <meta name="csrf-token" content="{{ csrf_token() }}">

        <title>{{ config('app.name', 'Laravel') }}</title>

        <!-- Fonts -->
        <link rel="preconnect" href="https://fonts.bunny.net">
        <link href="https://fonts.bunny.net/css?family=figtree:400,500,600&display=swap" rel="stylesheet" />

        <!-- Bootstrap CSS -->
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">

        <!-- Custom CSS -->
        <style>
            .bg-custom {
                background-color: #001f3f; /* 紺色の背景色 */
            }
            .nav-tabs {
                border-bottom: 1px solid #ddd;
            }
            .nav-tabs .nav-link {
                color: #fff; /* タブの文字色 */
                border: 1px solid #002f6c; /* タブの枠線の色 */
            }
            .nav-tabs .nav-link.active {
                background-color: #002f6c; /* アクティブタブの背景色 */
                color: #ffffff; /* アクティブタブの文字色 */
            }
            .tab-content {
                border-top: 1px solid #ddd;
                padding: 15px;
                background-color: #001f3f; /* タブコンテンツの背景色 */
            }
            .header-bg {
                background-color: #001f3f; /* ヘッダーの背景色 */
            }
            .text-custom {
                color: #fff; /* 全体の文字色 */
            }
        </style>

        <!-- Scripts -->
        @vite(['resources/css/app.css', 'resources/js/app.js'])
    </head>
    <body class="font-sans antialiased bg-custom text-custom">
        <div class="min-h-screen">
            @include('layouts.navigation')

            <!-- Page Heading -->
            @isset($header)
                <header class="header-bg shadow">
                    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
                        {{ $header }}
                    </div>
                </header>
            @endisset

            <!-- Page Content -->
            <main class="py-6">
                <div class="container mt-4">
                    <!-- ナビゲーションタブ -->
                    <ul class="nav nav-tabs" id="main-tabs">
                        <li class="nav-item">
                            <a class="nav-link active" href="#" data-tab="dashboard">Dashboard</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#" data-tab="userinfo">User Infos</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#" data-tab="notice">Notices</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#" data-tab="inquiry">Inquiries</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#" data-tab="present">Presents</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#" data-tab="present">Raid rooms
                            </a>
                        </li>
                    </ul>
                    
                    <!-- タブのコンテンツ -->
                    <div id="tab-content" class="mt-3">
                        @yield('tab-content')
                    </div>
                </div>
                <script>
                    document.addEventListener('DOMContentLoaded', function () {
                        const tabs = document.querySelectorAll('#main-tabs .nav-link');
                        const tabContent = document.getElementById('tab-content');
                    
                        // タブクリック時の処理
                        tabs.forEach(tab => {
                            tab.addEventListener('click', function (e) {
                                e.preventDefault();
                            
                                // アクティブなタブを切り替える
                                tabs.forEach(t => t.classList.remove('active'));
                                this.classList.add('active');
                            
                                // データ属性から対象タブを取得
                                const target = this.getAttribute('data-tab');
                            
                                // データ属性を用いてコンテンツを表示
                                loadTabContent(target);
                            });
                        });
                    
                        // 初期表示: Dashboardタブをロード
                        loadTabContent('dashboard');
                    
                        // タブコンテンツのロード関数
                        function loadTabContent(tab) {
                            fetch(`/api/tabs/${tab}`)
                                .then(response => response.text())
                                .then(html => {
                                    tabContent.innerHTML = html;
                                })
                                .catch(error => {
                                    tabContent.innerHTML = '<p>エラーが発生しました。コンテンツを読み込めません。</p>';
                                    console.error('Error:', error);
                                });
                        }
                    });
                </script>
            </main>
        </div>

        <!-- Bootstrap JS -->
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    </body>
</html>
