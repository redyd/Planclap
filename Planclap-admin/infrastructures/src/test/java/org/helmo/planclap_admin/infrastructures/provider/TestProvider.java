package org.helmo.planclap_admin.infrastructures.provider;

import org.helmo.planclap_admin.domains.core.*;

import java.util.*;

public class TestProvider {

    private final static Random R = new Random();

    public static Movie movie(int duration, boolean forChildren) {
        CineCheckAge age = forChildren ? CineCheckAge.AL : CineCheckAge.EIGHTEEN;

        return new Movie(
                Name.of("Movie%d".formatted(R.nextInt())),
                MovieDescription.of("Description%d".formatted(R.nextInt())),
                MovieDuration.of(duration),
                PosterURI.of("www.poster.com/%d/png".formatted(R.nextInt())),
                CineCheckGroup.of(age, List.of())
        );
    }

}
