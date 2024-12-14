<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Log;

class VerifyCsrfToken
{
    /**
     * Handle an incoming request.
     *
     * @param  \Illuminate\Http\Request  $request
     * @param  \Closure  $next
     * @return mixed
     */
    public function handle(Request $request, Closure $next)
    {
        // CSRFトークンがリクエストに含まれているか確認
        if ($request->isMethod('post') && !$request->has('_token') && !$this->isExcludedFromCsrf($request)) {
            return response('CSRF token missing or incorrect.', 419);
        }

        return $next($request);
    }

    /**
     * 特定のURLはCSRF検証をスキップする
     *
     * @param  \Illuminate\Http\Request  $request
     * @return bool
     */
    protected function isExcludedFromCsrf(Request $request)
    {
        $excludedRoutes = [
            // 'api/reply', // 例: APIルート
        ];

        foreach ($excludedRoutes as $route) {
            if ($request->is($route)) {
                return true;
            }
        }

        return false;
    }
}
