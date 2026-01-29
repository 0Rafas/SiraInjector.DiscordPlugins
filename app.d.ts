/**
 * Discord Streak Plugin
 *
 * A professional Discord plugin that tracks friendship streaks
 * Features:
 * - Track consecutive days of interaction between users
 * - Display streak count with fire emoji indicators
 * - Automatic streak reset after 24+ hours
 * - Beautiful Discord-native UI design
 *
 * @version 1.0.0
 */
declare class Logger {
    private prefix;
    constructor(moduleName: string);
    info(message: string): void;
    success(message: string): void;
    error(message: string): void;
}
interface Streak {
    userId: string;
    friendId: string;
    count: number;
    lastInteractionDate: number;
    createdDate: number;
}
declare class StreakManager {
    private streaks;
    private logger;
    constructor();
    /**
     * Update or create a streak
     */
    updateStreak(userId: string, friendId: string): void;
    /**
     * Get a specific streak
     */
    getStreak(userId: string, friendId: string): Streak | undefined;
    /**
     * Get all streaks
     */
    getAllStreaks(): Streak[];
    /**
     * Get statistics
     */
    getStatistics(): {
        totalStreaks: number;
        activeStreaks: number;
        maxStreak: number;
    };
}
declare class StreakPlugin {
    private streakManager;
    private logger;
    constructor();
    /**
     * Initialize the plugin
     */
    start(): Promise<void>;
    /**
     * Stop the plugin
     */
    stop(): void;
}
export { StreakPlugin, StreakManager, Logger };
//# sourceMappingURL=app.d.ts.map