/**
 * Discord Streak Plugin
 * A professional plugin that tracks friendship streaks
 */

// Simple Logger
class Logger {
    private prefix: string;

    constructor(moduleName: string) {
        this.prefix = `[${moduleName}]`;
    }

    info(message: string): void {
        console.log(`%c${this.prefix}%c ${message}`, "color: #5865f2; font-weight: bold;", "color: #dbdee1;");
    }

    success(message: string): void {
        console.log(`%c${this.prefix}%c ? ${message}`, "color: #43b581; font-weight: bold;", "color: #dbdee1;");
    }

    error(message: string): void {
        console.error(`%c${this.prefix}%c ? ${message}`, "color: #f04747; font-weight: bold;", "color: #dbdee1;");
    }

    warn(message: string): void {
        console.warn(`%c${this.prefix}%c ?? ${message}`, "color: #faa61a; font-weight: bold;", "color: #dbdee1;");
    }
}

// Streak Interface
interface Streak {
    userId: string;
    friendId: string;
    count: number;
    lastInteractionDate: number;
    createdDate: number;
}

// Streak Manager
class StreakManager {
    private streaks: Map<string, Streak> = new Map();
    private logger: Logger;

    constructor() {
        this.logger = new Logger("StreakManager");
    }

    updateStreak(userId: string, friendId: string): void {
        const key = `${userId}-${friendId}`;
        const now = Date.now();
        const oneDayMs = 24 * 60 * 60 * 1000;

        let streak = this.streaks.get(key);

        if (!streak) {
            streak = {
                userId,
                friendId,
                count: 1,
                lastInteractionDate: now,
                createdDate: now
            };
            this.logger.info(`?? New streak started: ${userId} <-> ${friendId}`);
        } else {
            const timeDiff = now - streak.lastInteractionDate;

            if (timeDiff < oneDayMs * 2) {
                streak.count++;
                this.logger.success(`Streak increased to ${streak.count} days!`);
            } else {
                streak.count = 1;
                this.logger.warn(`Streak reset (no interaction for >48h)`);
            }

            streak.lastInteractionDate = now;
        }

        this.streaks.set(key, streak);
    }

    getStreak(userId: string, friendId: string): Streak | undefined {
        const key = `${userId}-${friendId}`;
        return this.streaks.get(key);
    }

    getAllStreaks(): Streak[] {
        return Array.from(this.streaks.values());
    }

    getStatistics() {
        const allStreaks = Array.from(this.streaks.values());
        return {
            totalStreaks: this.streaks.size,
            activeStreaks: allStreaks.filter(s => s.count > 0).length,
            maxStreak: Math.max(...allStreaks.map(s => s.count), 0)
        };
    }
}

// Main Plugin Class
class StreakPlugin {
    private streakManager: StreakManager;
    private logger: Logger;
    private version = "1.0.0";

    constructor() {
        this.logger = new Logger("StreakPlugin");
        this.streakManager = new StreakManager();
    }

    async start(): Promise<void> {
        try {
            // Banner
            console.log("%c? Streak Plugin v" + this.version + " - Loaded Successfully!", "color: #5865f2; font-size: 14px; font-weight: bold;");
            
            this.logger.info("?? Initializing Streak Plugin...");
            
            // Create some test streaks
            this.streakManager.updateStreak("user1", "user2");
            this.streakManager.updateStreak("user3", "user4");
            this.streakManager.updateStreak("user5", "user6");
            
            this.logger.success("Plugin initialized successfully");
            
            // Log statistics
            const stats = this.streakManager.getStatistics();
            this.logger.info(`?? Statistics: ${JSON.stringify(stats)}`);
            
            // Show startup info
            this.logger.info("?? Tip: Use console to check streaks");
            
            console.log("%c? Streak Plugin Ready to Track! Type 'streakPlugin' in console for more info", "color: #43b581; font-size: 12px; font-weight: bold;");
            
        } catch (error) {
            this.logger.error(`Failed to initialize: ${String(error)}`);
        }
    }

    stop(): void {
        this.logger.info("?? Stopping Streak Plugin...");
    }

    // Public API for console testing
    getInfo() {
        return {
            version: this.version,
            stats: this.streakManager.getStatistics(),
            allStreaks: this.streakManager.getAllStreaks()
        };
    }
}

// Initialize and start the plugin
const streakPlugin = new StreakPlugin();
streakPlugin.start();

// Export for use in other modules
export { streakPlugin, StreakPlugin, StreakManager, Logger };
